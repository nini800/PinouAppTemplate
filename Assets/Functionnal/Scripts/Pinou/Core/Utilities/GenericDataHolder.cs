using System;

public class GenericDataHolder
{
    public GenericDataHolder() { }
    public GenericDataHolder(object genericData)
    {
        SetGenericData(0, genericData);
    }
    public GenericDataHolder(object[] genericData)
    {
        if (genericData.Length > 0)
        {
            for (int i = genericData.Length - 1; i >= 0; i--)
            {
                SetGenericData(i, genericData[i]);
            }
        }
    }
    public object GenericData { get => GetGenericData(0); set => SetGenericData(0, value); }
    public object GetGenericData(int id)
    {
        if (_genericDatas.Length <= id)
        {
            return null;
        }
        else
        {
            return _genericDatas[id];
        }
    }
    public T GetGenericData<T>(int id)
    {
        return (T)GetGenericData(id);
    }
    public void SetGenericData(int id, object value)
    {
        if (_genericDatas.Length <= id)
        {
            Array.Resize(ref _genericDatas, id + 1);
        }

        _genericDatas[id] = value;
    }
    private object[] _genericDatas = new object[0];
}