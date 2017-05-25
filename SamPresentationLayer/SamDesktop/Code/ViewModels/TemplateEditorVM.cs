using SamDesktop.Code.Constants;
using SamModels.DTOs;
using SamUtils.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SamDesktop.Code.ViewModels
{
    public class TemplateEditorVM : ViewModelBase
    {
        #region Ctors:
        public TemplateEditorVM()
        {

        }
        #endregion

        #region Fields:
        private int id;
        public int ID
        {
            get { return id; }
            set
            {
                id = value;
                RaisePropertyChanged("ID");
            }
        }

        private string name;
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                RaisePropertyChanged("Name");
            }
        }

        private TemplateCategoryDto templateCategory;
        public TemplateCategoryDto TemplateCategory
        {
            get { return templateCategory; }
            set
            {
                templateCategory = value;
                RaisePropertyChanged("TemplateCategory");
            }
        }

        private Image backgroundImage;
        public Image BackgroundImage
        {
            get { return backgroundImage; }
            set
            {
                backgroundImage = value;
                RaisePropertyChanged("BackgroundImage");
            }
        }

        private string text;
        public string Text
        {
            get { return text; }
            set
            {
                text = value;
                RaisePropertyChanged("Text");
            }
        }

        private double price;
        public double Price
        {
            get { return price; }
            set
            {
                price = value;
                RaisePropertyChanged("Price");
            }
        }

        private AspectRatio aspectRatio;
        public AspectRatio AspectRatio
        {
            get { return aspectRatio; }
            set
            {
                aspectRatio = value;
                RaisePropertyChanged("AspectRatio");
            }
        }

        private bool isActive;
        public bool IsActive
        {
            get { return isActive; }
            set
            {
                isActive = value;
                RaisePropertyChanged("IsActive");
            }
        }

        private ObservableCollection<TemplateFieldDto> fields;
        public ObservableCollection<TemplateFieldDto> Fields
        {
            get { return fields; }
            set
            {
                fields = value;
                RaisePropertyChanged("Fields");
            }
        }
        #endregion

        #region View Specific Props:
        public ObservableCollection<AspectRatio> AspectRatios
        {
            get
            {
                return new ObservableCollection<AspectRatio>(SamUtils.Constants.Collections.AspectRatios);
            }
        }

        private ObservableCollection<TemplateCategoryDto> templateCategories;
        public ObservableCollection<TemplateCategoryDto> TemplateCategories
        {
            get
            {
                return templateCategories;
            }
            set
            {
                templateCategories = value;
                RaisePropertyChanged("TemplateCategories");
            }
        }
        #endregion
    }
}
