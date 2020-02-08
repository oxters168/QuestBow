using UnityEngine;

public class PuppetEnemy : EnemyController
{
    public RootMotion.Dynamics.PuppetMaster puppet;

    public override void Awake()
    {
        base.Awake();
        selfTargetRoot = puppet.targetRoot;
    }

    public override void Spawn(Vector3 position, Quaternion rotation, System.Action onExpired = null)
    {
        puppet.Resurrect();
        puppet.Rebuild();
        puppet.Teleport(position, rotation, true);

        base.Spawn(position, rotation, onExpired);
    }

    public override void Kill()
    {
        puppet.Kill();

        base.Kill();
    }
}
