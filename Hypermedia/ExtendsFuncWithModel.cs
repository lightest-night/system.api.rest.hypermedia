using System;

#pragma warning disable 1591

namespace LightestNight.System.Api.Rest.Hypermedia
{
    public static class ExtendsFuncWithModel
    {
        public static Func<object, object> Downcast<TModel>(this Func<TModel, object> valueFunc)
            => value => valueFunc((TModel) value);
    }
}