using System;
using System.Collections.Generic;

namespace ReQube.Models
{
    public static class Constants
    {
        public const string EngineId = "R#";

        public const string SonarQubeCodeSmellType = "CODE_SMELL";

        public static IDictionary<string, string> ReSharperToSonarQubeSeverityMap { get; } = new Dictionary<string, string>
                                                                                                 {
                                                                                                     { "ERROR", "CRITICAL" },
                                                                                                     { "WARNING", "MAJOR" },
                                                                                                     { "SUGGESTION", "MINOR" },
                                                                                                     { "HINT", "INFO" },
                                                                                                     { "INFO", "INFO" }
                                                                                                 };

        // Extract from https://www.codeproject.com/Reference/720512/List-of-Visual-Studio-Project-Type-GUIDs
        public static IDictionary<string, Guid> ProjectTypeGuids { get; } = new Dictionary<string, Guid>
        {
            {"ASP.NET 5", new Guid("8BB2217D-0F2D-49D1-97BC-3654ED321F3B")},
            {"ASP.NET MVC 1", new Guid("603C0E0B-DB56-11DC-BE95-000D561079B0")},
            {"ASP.NET MVC 2", new Guid("F85E285D-A4E0-4152-9332-AB1D724D3325")},
            {"ASP.NET MVC 3", new Guid("E53F8FEA-EAE0-44A6-8774-FFD645390401")},
            {"ASP.NET MVC 4", new Guid("E3E379DF-F4C6-4180-9B81-6769533ABE47")},
            {"ASP.NET MVC 5", new Guid("349C5851-65DF-11DA-9384-00065B846F21")},
            {"C#", new Guid("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC")},
            {"C++", new Guid("8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942")},
            {"Database", new Guid("A9ACE9BB-CECE-4E62-9AA4-C7E7C5BD2124")},
            {"Database (other project types)", new Guid("4F174C21-8C12-11D0-8340-0000F80270F8")},
            {"Deployment Cab", new Guid("3EA9E505-35AC-4774-B492-AD1749C4943A")},
            {"Deployment Merge Module", new Guid("06A35CCD-C46D-44D5-987B-CF40FF872267")},
            {"Deployment Setup", new Guid("978C614F-708E-4E1A-B201-565925725DBA")},
            {"Deployment Smart Device Cab", new Guid("AB322303-2255-48EF-A496-5904EB18DA55")},
            {"Distributed System", new Guid("F135691A-BF7E-435D-8960-F99683D2D49C")},
            {"Dynamics 2012 AX C# in AOT", new Guid("BF6F8E12-879D-49E7-ADF0-5503146B24B8")},
            {"F#", new Guid("F2A71F9B-5D33-465A-A702-920D77279786")},
            {"J#", new Guid("E6FDF86B-F3D1-11D4-8576-0002A516ECE8")},
            {"Legacy (2003) Smart Device (C#)", new Guid("20D4826A-C6FA-45DB-90F4-C717570B9F32")},
            {"Legacy (2003) Smart Device (VB.NET)", new Guid("CB4CE8C6-1BDB-4DC7-A4D3-65A1999772F8")},
            {"Micro Framework", new Guid("b69e3092-b931-443c-abe7-7e7b65f2a37f")},
            {"Model-View-Controller v2 (MVC 2)", new Guid("F85E285D-A4E0-4152-9332-AB1D724D3325")},
            {"Model-View-Controller v3 (MVC 3)", new Guid("E53F8FEA-EAE0-44A6-8774-FFD645390401")},
            {"Model-View-Controller v4 (MVC 4)", new Guid("E3E379DF-F4C6-4180-9B81-6769533ABE47")},
            {"Model-View-Controller v5 (MVC 5)", new Guid("349C5851-65DF-11DA-9384-00065B846F21")},
            {"Mono for Android", new Guid("EFBA0AD7-5A72-4C68-AF49-83D382785DCF")},
            {"MonoTouch", new Guid("6BC8ED88-2882-458C-8E55-DFD12B67127B")},
            {"MonoTouch Binding", new Guid("F5B4F3BC-B597-4E2B-B552-EF5D8A32436F")},
            {"Portable Class Library", new Guid("786C830F-07A1-408B-BD7F-6EE04809D6DB")},
            {"Project Folders", new Guid("66A26720-8FB5-11D2-AA7E-00C04F688DDE")},
            {"SharePoint (C#)", new Guid("593B0543-81F6-4436-BA1E-4747859CAAE2")},
            {"SharePoint (VB.NET)", new Guid("EC05E597-79D4-47f3-ADA0-324C4F7C7484")},
            {"SharePoint Workflow", new Guid("F8810EC1-6754-47FC-A15F-DFABD2E3FA90")},
            {"Silverlight", new Guid("A1591282-1198-4647-A2B1-27E5FF5F6F3B")},
            {"Smart Device (C#)", new Guid("4D628B5B-2FBC-4AA6-8C16-197242AEB884")},
            {"Smart Device (VB.NET)", new Guid("68B1623D-7FB9-47D8-8664-7ECEA3297D4F")},
            {"Solution Folder", new Guid("2150E333-8FDC-42A3-9474-1A3956D46DE8")},
            {"Test", new Guid("3AC096D0-A1C2-E12C-1390-A8335801FDAB")},
            {"Universal Windows Class Library", new Guid("A5A43C5B-DE2A-4C0C-9213-0A381AF9435A")},
            {"VB.NET", new Guid("F184B08F-C81C-45F6-A57F-5ABD9991F28F")},
            {"Visual Database Tools", new Guid("C252FEB5-A946-4202-B1D4-9916A0590387")},
            {"Visual Studio 2015 Installer Project Extension", new Guid("54435603-DBB4-11D2-8724-00A0C9A8B90C")},
            {"Visual Studio Tools for Applications (VSTA)", new Guid("A860303F-1F3F-4691-B57E-529FC101A107")},
            {"Visual Studio Tools for Office (VSTO)", new Guid("BAA0C2D2-18E2-41B9-852F-F413020CAA33")},
            {"Web Application", new Guid("349C5851-65DF-11DA-9384-00065B846F21")},
            {"Web Site", new Guid("E24C65DC-7377-472B-9ABA-BC803B73C61A")},
            {"Windows (C#)", new Guid("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC")},
            {"Windows (VB.NET)", new Guid("F184B08F-C81C-45F6-A57F-5ABD9991F28F")},
            {"Windows (Visual C++)", new Guid("8BC9CEB8-8B4A-11D0-8D11-00A0C91BC942")},
            {"Windows Communication Foundation (WCF)", new Guid("3D9AD99F-2412-4246-B90B-4EAA41C64699")},
            {"Windows Phone 8/8.1 Blank/Hub/Webview App", new Guid("76F1466A-8B6D-4E39-A767-685A06062A39")},
            {"Windows Phone 8/8.1 App (C#)", new Guid("C089C8C0-30E0-4E22-80C0-CE093F111A43")},
            {"Windows Phone 8/8.1 App (VB.NET)", new Guid("DB03555F-0C8B-43BE-9FF9-57896B3C5E56")},
            {"Windows Presentation Foundation (WPF)", new Guid("60DC8134-EBA5-43B8-BCC9-BB4BC16C2548")},
            {"Windows Store (Metro) Apps & Components", new Guid("BC8A1FFA-BEE3-4634-8014-F334798102B3")},
            {"Workflow (C#)", new Guid("14822709-B5A1-4724-98CA-57A101D1B079")},
            {"Workflow (VB.NET)", new Guid("D59BE175-2ED0-4C54-BE3D-CDAA9F3214C8")},
            {"Workflow Foundation", new Guid("32F31D43-81CC-4C15-9DE6-3FC5453562B6")},
            {"Xamarin.Android", new Guid("EFBA0AD7-5A72-4C68-AF49-83D382785DCF")},
            {"Xamarin.iOS", new Guid("6BC8ED88-2882-458C-8E55-DFD12B67127B")},
            {"XNA (Windows)", new Guid("6D335F3A-9D43-41b4-9D22-F6F17C4BE596")},
            {"XNA (XBox)", new Guid("2DF5C3F4-5A5F-47a9-8E94-23B4456F55E2")},
            {"XNA (Zune)", new Guid("D399B71A-8929-442a-A9AC-8BEC78BB2433")},
        };

    }
}
