namespace Red.Data
{
    public interface ILoadable<in TFrom>
    {
        void Load(TFrom source);
    }
}