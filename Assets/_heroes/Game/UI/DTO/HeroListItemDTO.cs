namespace Heroes.Presentation.UI.HeroesPanel
{
    public sealed class HeroListItemDTO
    {
        public string Id;
        public string Name;
        public string Icon;
        public float Hp;
        public float MaxHp;

        public HeroListItemDTO(string id, string name, string icon, float hp, float maxHp)
        {
            Id = id;
            Name = name;
            Icon = icon;
            Hp = hp;
            MaxHp = maxHp;
        }
    }
}
