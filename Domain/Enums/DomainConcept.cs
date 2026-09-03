using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Enums
{
    public enum DomainConcept
    {
        #region - Product -
        Brand,
        Category,
        ProductType,
        Product,
        Feature,
        FeatureValue,
        ProductImage,
        Variant,
        #endregion

        #region - Identities -
        PendingVerification,
        #endregion

        #region - Value Objects -
        CategoryCode,
        ImageUrl,
        ProductCode,
        SKU,
        Slug,
        Seo,
        Tag,
        #endregion
    }
}
