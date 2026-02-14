module Lib.SystemInfoWindows

open System
open System.Diagnostics
open System.Net
open System.Runtime.InteropServices

open Types

open Microsoft.Win32

let getWindowsBuild () =
  try
    use key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion")
    match key with
    | null -> 0
    | k ->
      match k.GetValue("CurrentBuildNumber") with
      | :? string as s -> int s
      | _ -> 0
  with
  | _ -> 0

let getWindowsVersion () =
  try
    let version = Environment.OSVersion.Version
    let build = getWindowsBuild()
    if version.Major = 10 && build >= 22000 then
      "Windows 11"
    elif version.Major = 10 then
      "Windows 10"
    else
      Environment.OSVersion.VersionString
  with
  | _ -> "Windows"

let getKernel () =
  try
    Environment.OSVersion.VersionString
  with
  | _ -> ""

let getDistroName () = getWindowsVersion()

let getDistroId () = getWindowsVersion()

let getHostName () =
  try
    Dns.GetHostName()
  with
  | _ -> ""


[<Struct; StructLayout(LayoutKind.Sequential)>]
type MEMORYSTATUSEX =
  val mutable dwLength: uint32
  val mutable dwMemoryLoad: uint32
  val mutable ullTotalPhys: uint64
  val mutable ullAvailPhys: uint64
  val mutable ullTotalPageFile: uint64
  val mutable ullAvailPageFile: uint64
  val mutable ullTotalVirtual: uint64
  val mutable ullAvailVirtual: uint64
  val mutable ullAvailExtendedVirtual: uint64

[<DllImport("kernel32.dll", SetLastError = true)>]
extern bool GlobalMemoryStatusEx(MEMORYSTATUSEX& lpBuffer)

let getMemoryInfo () =
  try
    let mutable memStatus = MEMORYSTATUSEX()
    memStatus.dwLength <- uint32 (Marshal.SizeOf(memStatus))
    if GlobalMemoryStatusEx(&memStatus) then
      let totalGB = (decimal memStatus.ullTotalPhys / decimal (1024UL * 1024UL * 1024UL)).ToString("0.0")
      let availGB = (decimal memStatus.ullAvailPhys / decimal (1024UL * 1024UL * 1024UL)).ToString("0.0")
      $"{availGB} GB / {totalGB} GB"
    else
      ""
  with
  | _ -> ""

let getCPUModel () =
  try
    let wmiQuery = "SELECT Name FROM Win32_Processor"
    let searcher = new System.Management.ManagementObjectSearcher(wmiQuery)
    let collection = searcher.Get()
    
    let mutable cpuModel = ""
    for item in collection do
      cpuModel <- item["Name"].ToString()
    
    cpuModel
  with
  | _ -> ""

let getShell () =
  try
    let env = Environment.GetEnvironmentVariable("PROMPT")
    if isNull env then
      "CMD"
    else
      "PowerShell"
  with
  | _ -> "Unknown"

let getUser () =
  try
    Environment.UserName
  with
  | _ -> ""

let getUptime () =
  try
    let wmiQuery = "SELECT LastBootUpTime FROM Win32_OperatingSystem"
    let searcher = new System.Management.ManagementObjectSearcher(wmiQuery)
    let collection = searcher.Get()
    
    let mutable bootTime = DateTime.MinValue
    for item in collection do
      let bootTimeStr = item["LastBootUpTime"].ToString()
      if bootTimeStr.Length >= 14 then
        let year = int bootTimeStr.[0..3]
        let month = int bootTimeStr.[4..5]
        let day = int bootTimeStr.[6..7]
        let hour = int bootTimeStr.[8..9]
        let minute = int bootTimeStr.[10..11]
        let second = int bootTimeStr.[12..13]
        bootTime <- DateTime(year, month, day, hour, minute, second)
    
    if bootTime <> DateTime.MinValue then
      let uptime = DateTime.Now - bootTime
      let hours = int uptime.TotalHours
      let minutes = uptime.Minutes
      $"{hours}h{minutes}m"
    else
      ""
  with
  | _ -> ""

let getLocalIpAddress () =
  try
    let hostName = Dns.GetHostName()
    let addressList = Dns.GetHostEntry(hostName).AddressList |> List.ofArray

    match addressList.IsEmpty with
    | true -> ""
    | false ->
      addressList
      |> List.rev
      |> List.tryItem 2
      |> Option.map string
      |> Option.defaultValue ""
  with
  | _ -> ""

let systemInfo () : Info =
  { distroId = getDistroId ()
    distroName = getDistroName ()
    kernelName = getKernel ()
    shell = getShell ()
    user = getUser ()
    hostName = getHostName ()
    memInfo = getMemoryInfo ()
    cpuModel = getCPUModel ()
    localIp = getLocalIpAddress ()
    upTime = getUptime () }
