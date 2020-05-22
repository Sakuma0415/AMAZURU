using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class WaterFog : MonoBehaviour
{
	protected MeshRenderer waterFogObj;
    public Material FogMaterial;
	 float scaleFactor = 1;

	void OnEnable ()
	{
        waterFogObj = gameObject.GetComponent<MeshRenderer>();
        if (waterFogObj == null)
			Debug.LogError("Volume Fog Object must have a MeshRenderer Component!");
		
		//Note: In forward lightning path, the depth texture is not automatically generated.
		if (Camera.main.depthTextureMode == DepthTextureMode.None)
			Camera.main.depthTextureMode = DepthTextureMode.Depth;
		
        waterFogObj.material = FogMaterial;
		
	}

	void Update ()
	{
		float radius = (transform.lossyScale.x + transform.lossyScale.y + transform.lossyScale.z) / 6;
        Material mat = Application.isPlaying ? waterFogObj.material : waterFogObj.sharedMaterial;
		if (mat)
			mat.SetVector ("FogParam", new Vector4(transform.position.x, transform.position.y, transform.position.z, radius * scaleFactor));
	}
}
