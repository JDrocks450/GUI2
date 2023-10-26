using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI2.Content
{
    public static class SpritesheetManager
    {
        public enum Collection
        {
            BLOCKS,
            GUI,
        }        
        public static Texture2D BlockCollection
        {
            get; private set;
        }
        public static Texture2D GUICollection
        {
            get; private set;
        }
        static Dictionary<Collection, Texture2D> Collections = new Dictionary<Collection, Texture2D>() {
            { Collection.BLOCKS, BlockCollection },
            { Collection.GUI, GUICollection }
        };
        public static void LoadCollections(ContentManager Manager, string blockCollectionName = default, string guiCollectionName = default)
        {
            if (blockCollectionName == default)
                blockCollectionName = "BlockCollection";
            if (guiCollectionName == default)
                guiCollectionName = "GUICollection";
            BlockCollection = Manager.Load<Texture2D>(blockCollectionName);
            GUICollection = Manager.Load<Texture2D>(guiCollectionName);
            Collections[Collection.BLOCKS] = BlockCollection;
            Collections[Collection.GUI] = GUICollection;
        }
        public static Spritesheet GetByCollection(Collection collect)
        {
            return new Spritesheet(Collections[collect]);
        }
    }
}
