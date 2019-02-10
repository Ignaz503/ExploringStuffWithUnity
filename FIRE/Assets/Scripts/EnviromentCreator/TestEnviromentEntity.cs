using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnviromentEntity : EnviromentEntity
{
    [SerializeField] MeshRenderer meshRenderer = null;
    [SerializeField] Material deselect = null;
    [SerializeField] Material selectedMaterial = null;

    bool isSelected = false;

    protected override void ShowUI(ControllableEntity Activator)
    {
        //TODO NOT HERE
        if (isSelected)
        {
            isSelected = false;
            meshRenderer.material = deselect;
        }
        else
        {
            isSelected = true;
            meshRenderer.material = selectedMaterial;
        }
    }
}
