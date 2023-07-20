using System.IO;
using FlaxEditor;
using FlaxEditor.Content;
using FlaxEditor.CustomEditors;
using FlaxEditor.CustomEditors.Elements;
using FlaxEditor.Windows.Assets;
using FlaxEngine;
using FPS.Data.Guns;

namespace FPS.Utilities.Customs.Editors
{
    /// <summary>
    /// WeaponEditorWindow Script.
    /// </summary>
    public class WeaponEditorWindow : CustomEditorWindow
    {
        public override void Initialize(LayoutElementsContainer layout)
        {
            layout.Button("Create New Gun");
            var weaponsDataDir = Path.Combine(Globals.ProjectContentFolder, "Game/Equipments/Weapons/Data");

            var spacer = new SpaceElement();
            spacer.Spacer.Width = 100;
            foreach (var file in Directory.EnumerateFiles(weaponsDataDir, "*.json"))
            {
                var asset = Content.LoadAsync(file) as JsonAsset;
                var gun = asset.CreateInstance<Gun>();
                var assetItem = Editor.Instance.ContentDatabase.Find(asset.ID) as JsonAssetItem;
                var contentItem = Editor.Instance.ContentDatabase.Find(gun.GunPrefab.ID) as JsonAssetItem;
                var gunThumbnail = contentItem.Thumbnail;

                var groupElement = layout.Group(gun.Name);
                var colLayout = new HorizontalPanelElement();
                var innerRowLayout = new VerticalPanelElement();

                innerRowLayout.Panel.Width = 500;
                innerRowLayout.Label(gun.Name);
                var button = new ButtonElement();
                button.Button.Text = "Modify Gun";
                button.Button.Clicked += () => new JsonAssetWindow(Editor.Instance, assetItem).Show();
                innerRowLayout.AddElement(button);

                colLayout.Image(gunThumbnail);
                colLayout.AddElement(spacer);
                colLayout.AddElement(innerRowLayout);

                var rowLayout = layout.VerticalPanel();
                rowLayout.AddElement(colLayout);
                groupElement.AddElement(rowLayout);
            }
        }

        protected override void Deinitialize()
        {
            base.Deinitialize();
            ClearReferenceValueAll();
        }
    }
}
