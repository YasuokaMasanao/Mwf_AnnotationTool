using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace annotationTool;
public class AnnotationLabel : INotifyPropertyChanged {
    private bool IsSelected;
    public bool isSelected {
        get { return IsSelected; }
        set {
            if (IsSelected != value) {
                IsSelected = value;
                OnPropertyChanged("isSelected");
            }
        }
    }

    private string LabelName;
    public string labelName {
        get { return LabelName; }
        set {
            if (LabelName != value) {
                LabelName = value;
                OnPropertyChanged("labelName");
            }
        }
    }

    private string Color;
    public string color {
        get { return Color; }
        set {
            if (Color != value) {
                Color = value;

                var converter = new System.Windows.Media.BrushConverter();
                color16 = (System.Windows.Media.Brush)converter.ConvertFromString(Color);

                OnPropertyChanged("color");
            }
        }
    }

    private System.Windows.Media.Brush Color16;
    public System.Windows.Media.Brush color16 {
        get { return Color16; }
        set {
            if (Color16 != value) {
                Color16 = value;
                OnPropertyChanged("color16");
            }
        }
    }

    private int Id;
    public int id {
        get { return Id; }
        set {
            if (Id != value) {
                Id = value;
            }
        }
    }

    public int incId {
        get { return Id+1; }
        set {
            if (Id != value) {
                   Id = value;
            }
        }
    }


    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName) {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
