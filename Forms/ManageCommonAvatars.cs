﻿using Avatar_Explorer.Classes;

namespace Avatar_Explorer.Forms
{
    public partial class ManageCommonAvatars : Form
    {
        private readonly Main _mainForm;
        private static readonly Image FileImage = Image.FromStream(new MemoryStream(Properties.Resources.FileIcon));
        private CommonAvatar[] _commonAvatars;

        public ManageCommonAvatars(Main mainform)
        {
            _mainForm = mainform;
            _commonAvatars = _mainForm.CommonAvatars;
            InitializeComponent();

            foreach (var commonAvatar in _commonAvatars)
            {
                CommonAvatarsCombobox.Items.Add(commonAvatar.Name);
            }

            if (CommonAvatarsCombobox.Items.Count > 0)
            {
                CommonAvatarsCombobox.SelectedIndex = 0;
            }

            GenerateAvatarList();
            RefleshCommonAvatarButtonColor();
        }

        private void GenerateAvatarList()
        {
            AvatarList.Controls.Clear();
            var items = _mainForm.Items.Where(item => item.Type == ItemType.Avatar).ToList();
            if (items.Count == 0) return;
            items = items.OrderBy(item => item.Title).ToList();

            var index = 0;
            foreach (Item item in _mainForm.Items.Where(item => item.Type == ItemType.Avatar))
            {
                Button button = CreateAvatarButton(item, _mainForm.CurrentLanguage);
                button.Text = item.Title;
                button.Location = new Point(0, (70 * index) + 2);
                button.Tag = item.ItemPath;
                var commonAvatar = GetCommonAvatar(item.ItemPath);
                button.BackColor = commonAvatar != null
                    ? commonAvatar.Avatars.Contains(item.ItemPath)
                        ? Color.LightGreen
                        : Color.FromKnownColor(KnownColor.Control)
                    : Color.FromKnownColor(KnownColor.Control);

                AvatarList.Controls.Add(button);
                index++;
            }
        }

        private static Button CreateAvatarButton(Item item, string language)
        {
            CustomItemButton button = new CustomItemButton(875);
            button.ImagePath = item.ImagePath;
            button.Picture = File.Exists(item.ImagePath) ? Image.FromFile(item.ImagePath) : FileImage;
            button.TitleText = item.Title;
            button.AuthorName = Helper.Translate("作者: ", language) + item.AuthorName;
            button.ToolTipText = item.Title;

            button.Click += (_, _) =>
            {
                button.BackColor = button.BackColor == Color.LightGreen
                    ? Color.FromKnownColor(KnownColor.Control)
                    : Color.LightGreen;
            };

            return button;
        }

        private CommonAvatar? GetCommonAvatar(string? name) => string.IsNullOrWhiteSpace(name)
            ? null
            : _commonAvatars.FirstOrDefault(commonAvatar => commonAvatar.Name == name);

        private void RefleshCommonAvatarButtonColor()
        {
            foreach (Button button in AvatarList.Controls)
            {
                var commonAvatar = GetCommonAvatar(CommonAvatarsCombobox.Text);
                button.BackColor = commonAvatar != null
                    ? commonAvatar.Avatars.Contains(button.Tag?.ToString())
                        ? Color.LightGreen
                        : Color.FromKnownColor(KnownColor.Control)
                    : Color.FromKnownColor(KnownColor.Control);
            }
        }

        private void CommonAvatarsCombobox_TextChanged(object sender, EventArgs e) =>
            RefleshCommonAvatarButtonColor();

        private void DeleteSelectedGroupButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(CommonAvatarsCombobox.Text))
            {
                MessageBox.Show(Helper.Translate("削除する共通素体を選択してください。", _mainForm.CurrentLanguage),
                    Helper.Translate("エラー", _mainForm.CurrentLanguage), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var result = MessageBox.Show(Helper.Translate("本当に削除しますか？", _mainForm.CurrentLanguage),
                Helper.Translate("確認", _mainForm.CurrentLanguage), MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.No) return;

            var commonAvatar = GetCommonAvatar(CommonAvatarsCombobox.Text);
            if (commonAvatar == null) return;

            _commonAvatars = _commonAvatars.Where(ca => ca.Name != commonAvatar.Name).ToArray();
            MessageBox.Show(Helper.Translate("削除が完了しました。", _mainForm.CurrentLanguage),
                Helper.Translate("完了", _mainForm.CurrentLanguage), MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void AddButton_Click(object o, EventArgs e)
        {
            var name = CommonAvatarsCombobox.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                MessageBox.Show(Helper.Translate("追加、編集する共通素体を選択してください。", _mainForm.CurrentLanguage),
                    Helper.Translate("エラー", _mainForm.CurrentLanguage), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var commonAvatar = GetCommonAvatar(name);

            if (commonAvatar == null)
            {
                _commonAvatars = _commonAvatars.Append(new CommonAvatar
                {
                    Name = name,
                    Avatars = AvatarList.Controls.OfType<Button>()
                        .Where(button => button.BackColor == Color.LightGreen)
                        .Select(button => button.Tag.ToString())
                        .Where(tag => !string.IsNullOrWhiteSpace(tag))
                        .ToArray()
                }).ToArray();

                MessageBox.Show(Helper.Translate("共通素体名: " + name + "\n\n" + "共通素体データの追加が完了しました。", _mainForm.CurrentLanguage),
                    Helper.Translate("完了", _mainForm.CurrentLanguage), MessageBoxButtons.OK, MessageBoxIcon.Information);
                CommonAvatarsCombobox.Items.Add(name);
            }
            else
            {
                commonAvatar.Avatars = AvatarList.Controls.OfType<Button>()
                    .Where(button => button.BackColor == Color.LightGreen)
                    .Select(button => button.Tag.ToString())
                    .Where(tag => !string.IsNullOrWhiteSpace(tag))
                    .ToArray();

                MessageBox.Show(Helper.Translate("共通素体名: " + name + "\n\n" + "共通素体データの更新が完了しました。", _mainForm.CurrentLanguage),
                    Helper.Translate("完了", _mainForm.CurrentLanguage), MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            _mainForm.CommonAvatars = _commonAvatars;
            RefleshCommonAvatarButtonColor();
        }
    }
}
