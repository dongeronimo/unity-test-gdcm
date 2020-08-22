using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine.UI;
using System.Threading;
using System.Threading.Tasks;

struct Loren
{
    internal int id;
    internal string field;
};
[StructLayout(LayoutKind.Sequential)]
struct UnsafeLoren
{
    internal int id;
    internal unsafe char* field;
}
[StructLayout(LayoutKind.Sequential)]
struct ImageDescriptionStruct
{
    public unsafe char* patient;
    public unsafe char* study;
    public unsafe char* series;
    public unsafe char* filepath;
}
public class LibTomographer : MonoBehaviour
{
    [DllImport("tomographer")]
    private static extern void BeginLoadingImages(string dir);
    [DllImport("tomographer")]
    private static extern int GetNumberOfImagesInDirectory();
    [DllImport("tomographer")]
    private static extern void GetImageDescriptionList(System.IntPtr out_description);

    private unsafe void GetDescriptions(int numberOfImages)
    {
        int memSize = numberOfImages * Marshal.SizeOf(typeof(ImageDescriptionStruct));
        System.IntPtr dataPtr = Marshal.AllocHGlobal(memSize);
        GetImageDescriptionList(dataPtr);
        ImageDescriptionStruct* dataArr = (ImageDescriptionStruct*)dataPtr;
        for(int i=0; i<numberOfImages; i++)
        {
            ImageDescriptionStruct data = dataArr[i];
            string filename = Marshal.PtrToStringAnsi((System.IntPtr)data.filepath);
            Debug.Log(filename);
        }
        Marshal.FreeHGlobal(dataPtr);
    }
    async void Start()
    {
        //Path do storage da app
        string imageDirectory = Application.persistentDataPath;
        //Inicializa o diretorio assincronamente
        bool hasLoadedImagesInDll = await Task<bool>.Factory.StartNew(() =>
        {
            BeginLoadingImages(imageDirectory);
            return false;
        });
        //Pega a qtd de imagens no diretório
        int numberOfImages = GetNumberOfImagesInDirectory();
        //TODO: Faz a carga
        GetDescriptions(numberOfImages); 
        Debug.Log("number of imgs = " + numberOfImages.ToString());
    }
    /*
    public Text LoadProgressOutput;
    [DllImport("tomographer")]
    private static extern void Initialize(string c);
    public delegate void OnDllProgressCallback(double pct);
    [DllImport("tomographer")]
    private static extern void LoadImages(System.IntPtr onProgressCallback);
    [DllImport("tomographer")]
    private static extern int bar(string c);
    [DllImport("tomographer")]
    private static extern void woo(int sz, System.IntPtr loren);
    [DllImport("tomographer")]
    private static extern int HGDCM_GetNumberOfImagesInDirectory(string filepath);

    int LoadProgress = 0;
    Thread ImageDirectoryLoadThread = null;
    private void OnLoadProgress(double p)
    {
        LoadProgress = Mathf.RoundToInt((float)p*100);
    }
    //https://stackoverflow.com/questions/4558082/passing-an-struct-array-into-c-dll-from-c-sharp
    unsafe void Start()
    {
        Initialize(Application.persistentDataPath);//Inicializa o estado da dll
        Debug.Log("Initialized the dll");
        ImageDirectoryLoadThread = new Thread(() =>
        {
            Debug.Log("starting thread");
            Debug.Log("Loading...");
            OnDllProgressCallback progressCallback = OnLoadProgress;
            LoadImages(Marshal.GetFunctionPointerForDelegate(progressCallback));
            Debug.Log("Done");
        });
        ImageDirectoryLoadThread.Start();
        //LoadImages(Marshal.GetFunctionPointerForDelegate(progressCallback));
        //DEMO DE COMO PEGAR DADOS DE DENTRO DO C++
        List<Loren> lorens = new List<Loren>();
        int dataLen = 10;
        int memSize = dataLen * Marshal.SizeOf(typeof(UnsafeLoren));
        Debug.Log("memsize = " + memSize);
        System.IntPtr dataPtr = Marshal.AllocHGlobal(memSize);
        woo(dataLen, dataPtr);
        UnsafeLoren* dataArr = (UnsafeLoren*)dataPtr;
        for(int i=0; i<dataLen; i++)
        {
            UnsafeLoren data = dataArr[i];
            int id = data.id;
            char* cstr = data.field;
            string field = Marshal.PtrToStringAnsi((System.IntPtr)cstr);
            lorens.Add(new Loren() { id = id, field = field });
        }
        Marshal.FreeHGlobal(dataPtr);
        foreach (var l in lorens)
        {
            Debug.Log("loren = " + l.id + ", " + l.field);
        }
    }
    unsafe void Update()
    {
        LoadProgressOutput.text = LoadProgress + " %";
    }
    */
}
