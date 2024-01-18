using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;


[RequireComponent(typeof(ARTrackedImageManager))]
public class TrackedImageGameObjectPlacer : MonoBehaviour
{
    public List<GameObject> prefabs;

    private ARTrackedImageManager trackedImageManager_;
    private Dictionary<string, GameObject> spawnedObjects_ = new();
    
    private void Awake()
    {
        trackedImageManager_ = GetComponent<ARTrackedImageManager>();
    }

    private void OnEnable()
    {
        trackedImageManager_.trackedImagesChanged += OnTrackedImagesChange;
    }
    
    private void OnDisable()
    {
        trackedImageManager_.trackedImagesChanged -= OnTrackedImagesChange;
    }

    private void OnTrackedImagesChange(ARTrackedImagesChangedEventArgs _args)
    {
        // when a new image is detected
        foreach (ARTrackedImage image in _args.added)
        {
            var imageName = image.referenceImage.name;
            // find the corresponding prefab
            GameObject prefab = prefabs.Find(_p => _p.name == imageName);
            // only spawn if there is a corresponding prefab and it hasn't been spawned yet
            if (prefab == null || spawnedObjects_.ContainsKey(imageName)) continue;
            // instantiate the prefab
            GameObject go = Instantiate(prefab, image.transform);
            // store the instance in the dictionary
            spawnedObjects_.Add(imageName, go);
        }
        
        // when tracked images are updated
        foreach (ARTrackedImage image in _args.updated)
        {
            var imageName = image.referenceImage.name;
            // find the corresponding spawned object
            if (!spawnedObjects_.TryGetValue(imageName, out GameObject go)) continue;
            // the object is only active when the image is being tracked
            go.SetActive(image.trackingState == TrackingState.Tracking);
        }
        
        // when tracked images are removed
        foreach (ARTrackedImage image in _args.removed)
        {
            var imageName = image.referenceImage.name;
            // find the corresponding spawned object
            if (!spawnedObjects_.TryGetValue(imageName, out GameObject go)) continue;
            Destroy(go);
            spawnedObjects_.Remove(imageName);
        }
        
    }
}
