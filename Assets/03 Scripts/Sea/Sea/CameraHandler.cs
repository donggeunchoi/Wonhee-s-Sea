using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : CinemachineExtension
{
    [SerializeField] private float maxY = 0f;

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        if (stage == CinemachineCore.Stage.Body)
        {
            Vector3 pos = state.FinalPosition;

            if (pos.y > maxY)
                pos.y = maxY;

            state.RawPosition = pos;
        }
    }
}
