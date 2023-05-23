using UnityEngine;
using Web3Unity.Scripts.Library.Ethers.Providers;

public class JSONRPCTest : MonoBehaviour
{
    // Start is called before the first frame update
    async void Start()
    {
        var _jsonRPC = new JsonRpcProvider("https://mainnet.infura.io/v3/c2fb6db5ce0940a5b6fa9473d0ba2ca0");
        var block = await _jsonRPC.GetBalance("0x1Cb003BF7cAcce58186e9F1797358557c617E723");
        Debug.Log("Get Balance: " + block);

    }
}
