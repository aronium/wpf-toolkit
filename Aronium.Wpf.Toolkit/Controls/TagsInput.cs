using System.Windows;
using System.Windows.Controls;

namespace Aronium.Wpf.Toolkit.Controls
{
    [TemplatePart(Name = "PART_TextInput", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_ContentPanel", Type = typeof(WrapPanel))]
    public class TagsInput : ItemsControl
    {
        TextBox inputBox;
        WrapPanel contentPanel;

        static TagsInput()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(TagsInput), new FrameworkPropertyMetadata(typeof(TagsInput)));
        }

        public TagsInput()
        {
            Focusable = false;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            inputBox = this.Template.FindName("PART_TextInput", this) as TextBox;
            contentPanel = this.Template.FindName("PART_ContentPanel", this) as WrapPanel;

            inputBox.KeyDown += OnInputBoxKeyDown;
        }

        private void OnInputBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter || e.Key == System.Windows.Input.Key.Tab)
            {
                if (!string.IsNullOrWhiteSpace(inputBox.Text))
                {
                    AddTag();

                    e.Handled = true;
                }
            }
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new TagItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is TagItem;
        }

        private void AddTag()
        {
            this.Items.Add(inputBox.Text);
            inputBox.Clear();
        }
    }

}
