using HQ4P.Tools.ManNic.SettingMgmt.API;

namespace HQ4P.Tools.ManNic.SettingMgmt.API
{
    public abstract class SettingsEntry<T>
    {
        public SettingsEntry<T> Parent { get; set; }
        public uint SortId { get; set; }
        public EnumEntryType EntryType { get; set; }
        public T EntryValue { get; set; }

    }
}