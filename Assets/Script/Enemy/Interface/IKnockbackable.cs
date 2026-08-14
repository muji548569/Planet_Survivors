using UnityEngine;

public interface IKnockbackable
{
    public void ApplyKnockback(Vector3 sourcePosition, float knockbackMultiplier);
}
