using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Enumeration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace annotationTool;
class MwfReader {
    private static readonly Dictionary<int, string> tagToExplanation = new Dictionary<int, string>
        {
            {64, "Preamble"},
            {23, "Manufacturer"},
            {22, "Comment"},
            {130, "Patient Id"},
            {1, "Endianity"},
            {8, "Waveform"},
            {10, "gdftyp"},
            {4, "SPR"},
            {11, "SampleRate"},
            {12, "Cal"},
            {13, "Off"},
            {5, "NS"},
            {6, "Rec"},
            {63, "channel-specific settings"},
            {133, "Recording time"},
            {30, "data"},
            {9, "LeadId"},
            {129, "Patient Name"},
            {131, "Patient Age"},
            {132, "Patient Sex"},
        };

    public Dictionary<string, object>? MetaData { get; private set; } = null;
    public string? FilePath { get; private set; } = null;
    public ushort[]? Signal { get; private set; } = null;

    public MwfReader(string filename) {
        ParseMFERHeader(filename);
    }

    private void ParseMFERHeader(string filePath) {
        byte[]? buffer;
        MetaData = new Dictionary<string, object>();
        FilePath = null;


        using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read)) {

            while (stream.Position < stream.Length) {
                byte tag = (byte)stream.ReadByte();
                byte length = (byte)stream.ReadByte();

                if (tagToExplanation.ContainsKey(tag)) {
                    switch (tag) {
                        case 1: //"Endianity", # エンディアン 0でないならwarning
                            buffer = new byte[length];
                            stream.Read(buffer, 0, length);
                            int value = buffer[0];
                            if (value != 0 || length != 1) {
                                Console.WriteLine("Warning: The read byte is not zero. Check Endianity");
                            }
                            break;
                        case 8: //8: "Waveform", # 波形種別, 2が長時間心電図で,そうでないならwarning
                            buffer = new byte[length];
                            stream.Read(buffer, 0, length);
                            value = buffer[0];
                            if (value != 2 || length != 1) {
                                Console.WriteLine("Warning: The read byte is not 2. Check Waveform");
                            }
                            break;
                        case 10: //10: "gdftyp", # データタイプ, 1が16ビット符号なし整数で,そうでないならwarning
                            buffer = new byte[length];
                            stream.Read(buffer, 0, length);
                            value = buffer[0];
                            if (value != 1 || length != 1) {
                                Console.WriteLine("Warning: The read byte is not zero. Check gdftyp");
                            }
                            break;
                        case 4: //4: "SPR", # データブロック長, 1が固定で,そうでないならwarning
                            buffer = new byte[length];
                            stream.Read(buffer, 0, length);
                            value = buffer[0];
                            if (value != 1 || length != 1) {
                                Console.WriteLine("Warning: The read byte is not 1. Check SPR");
                            }
                            break;
                        case 11: // 11: "SampleRate", # サンプリング間隔, 1で秒，次が指数部（符号付き整数）, 次が整数部（符号なし整数）
                            buffer = new byte[length];
                            stream.Read(buffer, 0, length);
                            int[] intArray = new int[length];
                            for (int i = 0; i < length; i++) {
                                intArray[i] = buffer[i];
                            }
                            if (intArray[0] != 1 || intArray[1] != 253 || intArray[2] != 4) {
                                Console.WriteLine("Warning: The read byte are unexpected. Check SPR");
                            }
                            MetaData[tagToExplanation[tag]] = intArray[2] * 10 ^ -(intArray[1] - 256);
                            break;
                        case 12: // 12: "Cal", # サンプリング解像度, 0でV，次が指数部（符号付き整数）, 次が整数部（4バイト分の符号なし整数）
                            buffer = new byte[length];
                            stream.Read(buffer, 0, length);
                            intArray = new int[length];
                            for (int i = 0; i < length; i++) {
                                intArray[i] = buffer[i];
                            }
                            if (intArray[0] != 0 || intArray[1] != 250 || intArray[2] != 0 || intArray[3] != 0 || intArray[4] != 0 || intArray[5] != 4) {
                                Console.WriteLine("Warning: The read bytes are unexpected. Check Cal");
                            }
                            MetaData[tagToExplanation[tag]] = intArray[5] * 10 ^ (intArray[1] - 256);
                            break;
                        case 5: //5: "NS", # チャンネル数, 1でないならwarning
                            buffer = new byte[length];
                            stream.Read(buffer, 0, length);
                            value = buffer[0];
                            if (value != 1 || length != 1) {
                                Console.WriteLine("Warning: The read byte is not 1. Check Number of channels");
                            }
                            break;
                        case 6: //6: "Rec", # シーケンス数, 0でないならwarning
                            buffer = new byte[length];
                            stream.Read(buffer, 0, length);
                            value = buffer[0];
                            if (value != 0) {
                                Console.WriteLine("Warning: The read byte is not 0. Check Rec");
                            }
                            break;
                        case 63: //63: "channel-specific settings", #チャンネル属性
                            length = (byte)stream.ReadByte(); // 1 more byte for length
                            buffer = new byte[length];
                            stream.Read(buffer, 0, length);
                            break;
                        case 133: // 12: ""Recording time", YYYYMMDDhhmmss + milliseconds, microseconds
                            buffer = new byte[length];
                            stream.Read(buffer, 0, length);
                            byte[] y = new byte[] { buffer[1], buffer[0] }; // little endian
                            int year = BitConverter.ToUInt16(y, 0);
                            int month = buffer[2];
                            int day = buffer[3];
                            int hour = buffer[4];
                            int minute = buffer[5];
                            int second = buffer[6];  //ミリ秒，マイクロ秒はいったん切り捨てる．必要なら使う
                            DateTime recordingTime = new DateTime(year, month, day, hour, minute, second);
                            //string recordingTimeString = recordingTime.ToString("yyyy-MM-dd HH:mm:ss");
                            MetaData[tagToExplanation[tag]] = recordingTime;
                            break;
                        case 30: //30: "data", #データ, 以降2byteずつ
                            buffer = new byte[4]; //4byte分が本来はデータ長だが設定されていないことが多いので読み飛ばす
                            stream.Read(buffer, 0, 4);
                            //残りを読み込む
                            buffer = new byte[(int)(stream.Length - stream.Position)];
                            stream.Read(buffer, 0, (int)(stream.Length - stream.Position));
                            //2byteずつushortに変換
                            int blength = buffer.Length / 2;
                            ushort[] signals = new ushort[blength];
                            for (int i = 0; i < blength; i++) {
                                byte[] s = new byte[] { buffer[(2 * i) + 1], buffer[2 * i] }; // little endian
                                signals[i] = BitConverter.ToUInt16(s, 0);
                            }
                            MetaData[tagToExplanation[tag]] = signals;
                            break;
                        default:
                            // 64: "preamble", #ヘッダ
                            // 23: "Manufacturer", #機器情報
                            // 22: "Comment", #コメント
                            // 13: "Off", # オフセット, 使っていないようなので無視
                            //130: " Patient Id", # 患者ID 12byte 1:固定
                            buffer = new byte[length];
                            stream.Read(buffer, 0, length);
                            string stvalues = Encoding.ASCII.GetString(buffer);
                            MetaData[tagToExplanation[tag]] = stvalues;
                            break;
                    }
                } else {
                    //Console.WriteLine($"{tag} undefined_tag {length}");
                    MetaData = null;
                    throw new Exception("File error");
                }

            }
        }
        FilePath = filePath;
        Signal = (ushort[])MetaData["data"];
        MetaData.Remove("data");
    }
}

