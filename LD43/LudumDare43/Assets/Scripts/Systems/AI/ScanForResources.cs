using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ScanForResources : MonoBehaviour
{

    public List<WorldResource> Scan(WorldResource.ResourceType type, float scanRange)
    {

        Collider[] hitColliders = Physics.OverlapSphere(transform.position, scanRange);
        List<WorldResource> targetResources = new List<WorldResource>();
        foreach (Collider collider in hitColliders)
        {
            WorldResource resource = collider.GetComponent<WorldResource>();
            if (resource != null && type == resource.type)
            {
                targetResources.Add(resource);
            }
        }

        targetResources = targetResources.OrderBy(x => Vector2.Distance(transform.position, x.transform.position)).ToList();

        return targetResources;

    }
}
