using BrutileArcGIS.commands;
using ESRI.ArcGIS.ADF.BaseClasses;

namespace BrutileArcGIS.MenuDefs
{
    public class YandexMenuDef : BaseMenu
    {
        public YandexMenuDef()
        {
            m_barCaption = "Yandex";
            AddItem(typeof(AddYandexStreetsLayerCommand));
            AddItem(typeof(AddYandexSatelliteLayerCommand));
            AddItem(typeof(AddYandexHybridLayerCommand));
        }
    }
}
