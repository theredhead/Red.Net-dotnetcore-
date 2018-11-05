namespace Red.Data
{
    public interface ISavable<in TTo>
    {
        void Save(TTo target);
    }
}