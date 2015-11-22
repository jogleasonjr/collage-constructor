# collage-constructor
Create photo collages from multiple source images

```csharp
static void Main(string[] args)
{
  // IBuilderService is injected with a Google Image repository to
  // obtain images in this example. See UnityBootstrapper.cs.
  var builder = Container.Resolve<IBuilderService>();
  
  var buildTask = builder.Build("owl",
                                imagesCount: 64,
                                imageSize: 256,
                                outputColumns: 8,
                                outputRows: 8);
  
  var outputFile = buildTask.Result;
}
```

![ScreenShot](https://raw.githubusercontent.com/jogleasonjr/collage-constructor/master/images/owl_example.png)
