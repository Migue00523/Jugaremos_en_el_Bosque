
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public class BushHidingSpot : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform hidePosition;

    [Header("Character Spacing")]
    [SerializeField] private float spacing = 0.65f;

    private int assignedCharacters = 0;

    private static List<BushHidingSpot> allBushes =
        new List<BushHidingSpot>();

    private void OnEnable()
    {
        if (!allBushes.Contains(this))
            allBushes.Add(this);
    }

    private void OnDisable()
    {
        allBushes.Remove(this);
    }

    public static BushHidingSpot FindClosestBush(Vector3 position)
    {
        BushHidingSpot closest = null;
        float closestDistance = Mathf.Infinity;

        foreach (BushHidingSpot bush in allBushes)
        {
            if (bush == null || bush.hidePosition == null)
                continue;

            float distance = Vector3.Distance(
                position,
                bush.hidePosition.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = bush;
            }
        }

        return closest;
    }

    public Vector3 GetNextPosition()
    {
        int slot = assignedCharacters;
        assignedCharacters++;

        float angle = slot * 137.5f * Mathf.Deg2Rad;
        float radius = spacing * Mathf.Sqrt(slot);

        Vector3 offset = new Vector3(
            Mathf.Cos(angle) * radius,
            0f,
            Mathf.Sin(angle) * radius
        );

        Vector3 desiredPosition = hidePosition.position + offset;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(
            desiredPosition,
            out hit,
            2f,
            NavMesh.AllAreas))
        {
            return hit.position;
        }

        return hidePosition.position;
    }
}
