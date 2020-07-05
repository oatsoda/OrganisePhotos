using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace OrganisePhotos.App
{
    public static class InvokeExtension
    {
        public static void InvokeIfRequired<T>(this T control, MethodInvoker action, bool block = false) where T : ISynchronizeInvoke
        {
            var source = (ISynchronizeInvoke)control;

            if (source.InvokeRequired)
            {
                if (block)
                    source.Invoke(action, null);
                else
                    source.BeginInvoke(action, null);
            }
            else
            {
                action();
            }
        }
    }

    public static class ComboBoxExtensions
    {
        public static void BindToEnum<T>(this ComboBox comboBox) where T : struct, Enum
        {
            comboBox.DataSource = (T[])Enum.GetValues(typeof(T));
                                               // .Cast<T>()
                                               //      .Select(p => new { Key = p, Value = p.ToString() })
                                               //      .ToList();

            comboBox.DisplayMember = "";// "Value";
            comboBox.ValueMember = "";// "Key";
        }

        public static T SelectedEnum<T>(this ComboBox comboBox) where T : struct, Enum
        {
            return (T)comboBox.SelectedItem;
        }
    }
}
