using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MyFinance.MarkupExtensions
{
    public class FromFileImage : IMarkupExtension
    {
        public string FileName { get; set; }

        public object ProvideValue(IServiceProvider serviceProvider)
        {
            if (string.IsNullOrEmpty(FileName))
            {
                throw new Exception("FileName not provided");
            }

            var image = ImageSource.FromFile(FileName);
            return image;
        }
    }
}
