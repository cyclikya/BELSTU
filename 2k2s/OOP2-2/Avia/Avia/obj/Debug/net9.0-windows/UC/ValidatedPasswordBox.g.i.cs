﻿#pragma checksum "..\..\..\..\UC\ValidatedPasswordBox.xaml" "{ff1816ec-aa5e-4d10-87f7-6f4963833460}" "96E2C3C7EEF2735A04C8D30B68C0440ABFE6384E"
//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан программой.
//     Исполняемая версия:4.0.30319.42000
//
//     Изменения в этом файле могут привести к неправильной работе и будут потеряны в случае
//     повторной генерации кода.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace Avia.UC {
    
    
    /// <summary>
    /// ValidatedPasswordBox
    /// </summary>
    public partial class ValidatedPasswordBox : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 4 "..\..\..\..\UC\ValidatedPasswordBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal Avia.UC.ValidatedPasswordBox root;
        
        #line default
        #line hidden
        
        
        #line 18 "..\..\..\..\UC\ValidatedPasswordBox.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.PasswordBox PART_PasswordBox;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.3.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/Avia;component/uc/validatedpasswordbox.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\UC\ValidatedPasswordBox.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "9.0.3.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.root = ((Avia.UC.ValidatedPasswordBox)(target));
            return;
            case 2:
            this.PART_PasswordBox = ((System.Windows.Controls.PasswordBox)(target));
            
            #line 19 "..\..\..\..\UC\ValidatedPasswordBox.xaml"
            this.PART_PasswordBox.PasswordChanged += new System.Windows.RoutedEventHandler(this.PART_PasswordBox_PasswordChanged);
            
            #line default
            #line hidden
            
            #line 20 "..\..\..\..\UC\ValidatedPasswordBox.xaml"
            this.PART_PasswordBox.LostFocus += new System.Windows.RoutedEventHandler(this.PART_PasswordBox_LostFocus);
            
            #line default
            #line hidden
            
            #line 21 "..\..\..\..\UC\ValidatedPasswordBox.xaml"
            this.PART_PasswordBox.KeyDown += new System.Windows.Input.KeyEventHandler(this.PART_PasswordBox_KeyDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

