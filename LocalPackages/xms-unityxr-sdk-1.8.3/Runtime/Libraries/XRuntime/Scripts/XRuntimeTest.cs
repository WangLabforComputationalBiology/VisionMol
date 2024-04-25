using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.InteropServices;
using Ximmerse.XR;

public class XRuntimeTest : MonoBehaviour
{
    static System.Int64 sPlugin = 0;
    public struct TestDataA
    {
        public long t;
        public long v;
    };

    static int mem_size = 0;


    static System.IntPtr mem_a;

    void Start()
    {
        XRuntimeClient.Init();

        sPlugin = XRuntimeClient.xruntime_client_get_plugin("SampleA");

        Debug.LogFormat("get plugin: {0}\n", sPlugin);

        if (sPlugin > 0)
        {
            XRuntimeClient.xplugin_start(sPlugin);
            //XRuntimeClient.xplugin_subscribe_data(sPlugin, 0, callback, (System.IntPtr)null);
        }

        mem_a = Marshal.AllocHGlobal(Marshal.SizeOf<TestDataA>());
    }
   

    // Update is called once per frame
    void Update()
    {
        if (sPlugin > 0)
        {

            // sizeof TestDataA: 16
            // callback: 741142e680:0, 8, 0
            // get shm p=496274391040 size: 148
            //498505803392
            //System.IntPtr mem_a = XRuntimeClient.xplugin_get_output_shm(sPlugin, "test_mem_a", ref mem_size);
           
            int ret = XRuntimeClient.xplugin_get_ringbuf_data(sPlugin, "test_mem_a",0, 1,  mem_a);
            Debug.LogFormat("get shm p={0}\n", mem_a );
            if (ret >= 0)
            {
                //RB 0 2 1 64
                TestDataA testData = Marshal.PtrToStructure<TestDataA>(mem_a);
                Debug.LogFormat("TestDataA: {0},{1}\n", testData.t, testData.v);
            }
        }
    }

    private void OnApplicationPause(bool pause)
    {
        
    }


    private void OnApplicationQuit()
    {
        if (sPlugin > 0)
        {
            XRuntimeClient.xplugin_stop(sPlugin);
        }

        XRuntimeClient.Exit();

        Marshal.FreeHGlobal(mem_a);
    }
}
