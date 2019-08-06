using Terraria;
using Terraria.UI;
using Terraria.GameContent.UI.Elements;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace CustomNPCNames.UI
{
    public class UINPCPreview : UIElement
    {
        public UIImage npcHeadPreview;
        public UIImage npcGenderPreview;

        public UINPCPreview()
        {
            npcHeadPreview = new UIImage(ModContent.GetTexture("Terraria/NPC_Head_0"));
            Append(npcHeadPreview);
            npcGenderPreview = new UIImage(ModContent.GetTexture("Terraria/NPC_Head_0"));
            npcGenderPreview.Left.Set(35, 0);
            npcGenderPreview.Top.Set(0, 0);
            Append(npcGenderPreview);
        }

        public void UpdateNPC(short id)
        {
            npcHeadPreview.SetImage(RenameUI.GetNPCHeadTexture(id));
            npcHeadPreview.Left.Set(((30 - npcHeadPreview.Width.Pixels) / 2f) + CustomNPCNames.npcHeadOffset[id].X, 0);
            npcHeadPreview.Top.Set(((30 - npcHeadPreview.Height.Pixels) / 2f) + CustomNPCNames.npcHeadOffset[id].Y - 4, 0);
            npcGenderPreview.SetImage(NPCs.CustomNPC.isMale[id] ? ModContent.GetTexture("CustomNPCNames/UI/MaleIcon") : ModContent.GetTexture("CustomNPCNames/UI/FemaleIcon"));
            npcGenderPreview.Top.Set(NPCs.CustomNPC.isMale[id] ? 0 : -2, 0);
        }
    }
}
