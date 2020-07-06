using System;
using System.Windows.Forms;

namespace OrganisePhotos.App
{
    public static class ComboBoxExtensions
    {
        public static void BindToEnum<T>(this ComboBox comboBox) where T : struct, Enum
        {
            comboBox.BeginUpdate();
            comboBox.DataSource = (T[])Enum.GetValues(typeof(T));
            comboBox.DisplayMember = "";
            comboBox.ValueMember = "";
            comboBox.EndUpdate();
        }

        public static T SelectedEnum<T>(this ComboBox comboBox) where T : struct, Enum
        {
            if (comboBox.SelectedItem == null)
                return default;

            return (T)comboBox.SelectedItem;
        }
    }
}