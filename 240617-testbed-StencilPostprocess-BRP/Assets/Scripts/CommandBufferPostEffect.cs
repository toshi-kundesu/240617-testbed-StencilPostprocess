using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
// [ExecuteInEditMode]
public class CommandBufferPostEffect : MonoBehaviour {

    [SerializeField]
    private Shader _shader;
    [SerializeField]
    private bool _isActive = true;

    private void Awake () {
        
        Initialize();
    }

    // private void OnValidate() {
    //     if (_isActive) {
    //         Initialize();
    //     }
    // }

    private void Initialize()
    {
        var camera = GetComponent<Camera>();
        if (camera.allowMSAA) {
            Debug.LogError("MSAAがONになっていると正常に動作しない");
            // MSAAがONになっていると正常に動作しない
            return;
        }
        var material = new Material(_shader);
        var commandBuffer = new CommandBuffer();
        
        // 一時テクスチャを取得する
        // テクスチャのIDを取得するにはShader.PropertyToIDを使う
        int tempTextureIdentifier = Shader.PropertyToID("_PostEffectTempTexture");
        commandBuffer.GetTemporaryRT(tempTextureIdentifier, -1, -1);

        // 現在のレンダーターゲットを一時テクスチャにコピー
        // 一時テクスチャからレンダーターゲットにポストエフェクトを掛けつつ描画
        commandBuffer.Blit(BuiltinRenderTextureType.CurrentActive, tempTextureIdentifier);
        commandBuffer.Blit(tempTextureIdentifier, BuiltinRenderTextureType.CurrentActive, material);

        // 一時テクスチャを解放
        commandBuffer.ReleaseTemporaryRT(tempTextureIdentifier);

        camera.AddCommandBuffer(CameraEvent.AfterEverything, commandBuffer);
    }
}