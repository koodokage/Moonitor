internal interface ISaveableScript
{
    public string FileName { get;}
    public void Reset();

    public void Saving();

    public void Loading();

    public void Delete();

}