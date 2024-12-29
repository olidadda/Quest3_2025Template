-Basic usage: 
    -Use menu "GameObject/Procedutal Primitives/your primitive" to create a new primitive.
    -Adjust primitive's parameters in inspector.

-Scripting example:
    //Create a sphere
    GameObject shpereObj = ProceduralPrimitives.CreatePrimitive(ProceduralPrimitiveType.Sphere);
    Sphere m_sphere = shpereObj.GetComponent<Sphere>();
    
    //adjust sphere
    m_sphere.radius = 1.5f;
    m_sphere.sliceOn = true;
    m_sphere.sliceFrom = 90.0f;
    m_sphere.Apply(); //apply changes

-Others:
	-Use "Combiner" to combine multiple meshes into one single mesh.
	-Use "Instance" to create primitive instance that share the same mesh.
	-Click "Save mesh" in inspector to save current primitive mesh asset, the asset will be saved under "Assets/Procedural Primitives" folder.
	-Enter "PRIMITIVE_EDGES" in "Player Settings -> Other Settings -> Scripting Define Symbols" to enable special Apply() function that return construction edges.