using Glacier.Common.Provider;
using Glacier.Common.Util;
using GUI2.Content;
using GUI2.Controls;
using GUI2.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GUI2
{
    public class GUIProvider : IProvider
    {
        private GlacierSpriteBatch[] batches = new GlacierSpriteBatch[4];
        public ProviderManager Parent { get; set; }
        Dictionary<string, GLayer> Layers = new Dictionary<string, GLayer>();        
        public GLayer Default => Layers.Values.First();
        public GUIProvider(GLayer.FilteringModes DefaultLayerMode = GLayer.FilteringModes.NearestNeighborClamp)
        {
            InterfaceFont.LoadFonts();
            Create("root", 0);            
        }

        public GLayer Create(string name, float ZIndex, 
            GLayer.FilteringModes LayerMode = GLayer.FilteringModes.NearestNeighborClamp) => 
            Add(new GLayer(ZIndex) { Name = name, FilteringMode = LayerMode}); 

        public GLayer Get(string name)
        {
            if (Layers.TryGetValue(name, out var layer))
                return layer;
            else return null;
        }

        public GLayer Add(GLayer layer)
        {
            if (Get(layer.Name) == null)
            {
                Layers.Add(layer.Name, layer);
            }
            return layer;
        }

        public void Refresh(GameTime time)
        {
            foreach (var layer in Layers.Values)
                layer.Update(time);
        }

        public void Draw(GraphicsDevice device)
        {
            int drawnLayers = 0;
            for (int i = 0; i <= (int)GLayer.FilteringModes.NearestNeighborRepeating; i++)
            {
                GLayer.FilteringModes mode = (GLayer.FilteringModes)i;
                var layers = Layers.Values.Where(x => x.FilteringMode == mode);
                if (layers.Any()) {
                    var batch = GetSpritebatch(mode);
                    batch.Begin();
                    foreach (var layer in layers)
                    {
                        layer.Draw(batch);
                        drawnLayers++;
                    }
                    batch.End();
                }
                if (drawnLayers == Layers.Count) break;
            }
        }

        private GlacierSpriteBatch GetSpritebatch(GLayer.FilteringModes mode)
        {
            if (batches[(int)mode] == null)
                switch (mode)
                {
                    case GLayer.FilteringModes.BilinearClamp:
                        batches[(int)mode] = new GlacierSpriteBatch(GameResources.Device) { SampleState = SamplerState.LinearClamp };
                        break;
                    case GLayer.FilteringModes.BilinearRepeating:
                        batches[(int)mode] = new GlacierSpriteBatch(GameResources.Device) { SampleState = SamplerState.LinearWrap };
                        break;
                    case GLayer.FilteringModes.NearestNeighborClamp:
                        batches[(int)mode] = new GlacierSpriteBatch(GameResources.Device) { SampleState = SamplerState.PointClamp };
                        break;
                    case GLayer.FilteringModes.NearestNeighborRepeating:
                        batches[(int)mode] = new GlacierSpriteBatch(GameResources.Device) { SampleState = SamplerState.PointWrap };
                        break;
                }
            return batches[(int)mode];
        }
    }
}
