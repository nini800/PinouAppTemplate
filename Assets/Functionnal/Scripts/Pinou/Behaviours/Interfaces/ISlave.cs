public interface ISlave
{
    void SlaveAwake();
    void SlaveStart();
    void SlaveFixedUpdate();
    void SlaveUpdate();
    void SlaveDrawGizmos();
}

public interface IAdvancedSlave : ISlave
{
    void SlaveAfterFixedUpdate();
    void SlaveLateUpdate();
    void SlaveEnabled();
    void SlaveDisabled();
    void SlaveDestroyed();
}
