/*************************************************
**
** You're viewing a file in the SMAPI mod dump, which contains a copy of every open-source SMAPI mod
** for queries and analysis.
**
** This is *not* the original file, and not necessarily the latest version.
** Source repository: https://github.com/lshtech/DialogueExtension
**
*************************************************/

using System;
using JetBrains.Annotations;
using LightInject;
using StardewValley;

namespace DialogueExtension.CodeTester
{
  internal static class Program
  {
    private static void Main()
    {
      //var c = CreateContainer();
      //var factory = c.GetInstance<IWrapperFactory>();
      //var tc = factory.CreateInstance<ITestClass>(90);
      
      Console.WriteLine(typeof(Character).IsAssignableFrom(typeof(NPC)));
      Console.ReadLine();
    }

    [NotNull]
    private static ServiceContainer CreateContainer()
    {
      var container = new ServiceContainer();
      container.Register<ITestProperty, TestProperty>();
      container.Register<ITestClass, TestClass>();
      container.Register<ITestFactory, TestFactory>();
      //container.RegisterInstance(container);
      container.Register<IWrapperFactory, WrapperFactory>();
      container.RegisterInstance<IServiceFactory>(container.BeginScope());
      //container.Register<IServiceFactory, IWrapperFactory>();
      return container;
    }
  }

  

  public class WrapperFactory : IWrapperFactory
  {
    public TInterface CreateInstance<TInterface>()
      => (TInterface)_serviceFactory.GetInstance(typeof(TInterface));

    public TInterface CreateInstance<TInterface>(object item)
      => (TInterface)_serviceFactory.GetInstance(typeof(TInterface), new[] { item });

    public TInterface CreateInstance<TInterface>(params object[] args)
      => (TInterface)_serviceFactory.GetInstance(typeof(TInterface), args);

    private readonly IServiceFactory _serviceFactory;
    public WrapperFactory(IServiceFactory factory) => _serviceFactory = factory;
    public WrapperFactory() => _serviceFactory = new ServiceContainer();
  }

  public interface IWrapperFactory
  {
    TInterface CreateInstance<TInterface>();
    TInterface CreateInstance<TInterface>(object item);
    TInterface CreateInstance<TInterface>(params object[] args);
  }


  public class TestFactory : ITestFactory
  {
    public ITestClass GetTestClass()
    {
      return new TestClass(70);
    }

    private Func<ITestClass> _createTestClass;

    public TestFactory(Func<ITestClass> createTestClass)
    {
      _createTestClass = createTestClass;
    }
  }

  public interface ITestFactory
  {
    ITestClass GetTestClass();
  }
  
  public class TestClass : ITestClass
  {
    public ITestProperty TestProperty { get; set; }
    public int Value { get; set; }
    public TestClass() : this(54){}
    public TestClass(int value)
    {
      Value = value;
    }

    public int GetValue() => 27;
  }

  public interface ITestClass
  {
    ITestProperty TestProperty { get; set; }
    int Value { get; set; }
  }

  public class TestProperty : ITestProperty
  {
    
    public IServiceFactory Factory { get; set; } 
    public int Value { get; set; } = 90;
  }

  public interface ITestProperty
  {
    int Value { get; set; }
    IServiceFactory Factory { get; set; }
  }
 
}