using FlaxEditor.CustomEditors;
using FlaxEditor.CustomEditors.Editors;
using FlaxEngine;
using FPS.Combat;

namespace FPS.Utilities.Customs.Editors
{
    /// <summary>
    /// WeaponManagerScriptEditor Script.
    /// </summary>
    [CustomEditor(typeof(WeaponManager))]
    public class WeaponManagerScriptEditor : GenericEditor
    {
        public override void Initialize(LayoutElementsContainer layout)
        {
            base.Initialize(layout);

            layout.Space(20);
            var button = layout.Button("Open Weapons Window", Color.YellowGreen);
            button.Button.Clicked += () =>
            {
                var weaponEditor = new WeaponEditorWindow();
                weaponEditor.Show();
            };
        }
    }
}
