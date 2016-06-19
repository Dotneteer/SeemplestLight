// Copyright © GalaSoft Laurent Bugnion 2011-2016
// See license.txt in this project or http://www.galasoft.ch/license_MIT.txt
// 
// Refactored by Istvan Novak

using System;

namespace SeemplestLight.PortableCore.IoC
{
    /// <summary>
    /// When used with ServiceRegistry, specifies which constructor
    /// should be used to instantiate when GetInstance is called.
    /// If there is only one constructor in the class, this attribute is
    /// not needed.
    /// </summary>
    [AttributeUsage(AttributeTargets.Constructor)]
    public sealed class PreferredConstructorAttribute : Attribute
    {
    }
}