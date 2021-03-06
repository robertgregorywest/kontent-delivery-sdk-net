﻿using System;
using FakeItEasy;
using Kentico.Kontent.Delivery.Abstractions;
using Kentico.Kontent.Delivery.Abstractions.InlineContentItems;
using Kentico.Kontent.Delivery.Abstractions.RetryPolicy;
using Kentico.Kontent.Delivery.ContentLinks;
using RichardSzalay.MockHttp;

namespace Kentico.Kontent.Delivery.Tests.Factories
{
    internal static class DeliveryClientFactory
    {
        private static readonly MockHttpMessageHandler MockHttp = new MockHttpMessageHandler();
        private static IModelProvider _mockModelProvider = A.Fake<IModelProvider>();
        private static IPropertyMapper _mockPropertyMapper = A.Fake<IPropertyMapper>();
        private static IRetryPolicyProvider _mockResiliencePolicyProvider = A.Fake<IRetryPolicyProvider>();
        private static ITypeProvider _mockTypeProvider = A.Fake<ITypeProvider>();
        private static IContentLinkUrlResolver _mockContentLinkUrlResolver = A.Fake<IContentLinkUrlResolver>();
        private static IInlineContentItemsProcessor _mockInlineContentItemsProcessor = A.Fake<IInlineContentItemsProcessor>();

        internal static DeliveryClient GetMockedDeliveryClientWithProjectId(
            Guid projectId,
            MockHttpMessageHandler httpMessageHandler = null,
            IModelProvider modelProvider = null,
            IPropertyMapper propertyMapper = null,
            IRetryPolicyProvider resiliencePolicyProvider = null,
            ITypeProvider typeProvider = null,
            IContentLinkUrlResolver contentLinkUrlResolver = null,
            IInlineContentItemsProcessor inlineContentItemsProcessor = null
        )
        {
            if (modelProvider != null) _mockModelProvider = modelProvider;
            if (propertyMapper != null) _mockPropertyMapper = propertyMapper;
            if (resiliencePolicyProvider != null) _mockResiliencePolicyProvider = resiliencePolicyProvider;
            if (typeProvider != null) _mockTypeProvider = typeProvider;
            if (contentLinkUrlResolver != null) _mockContentLinkUrlResolver = contentLinkUrlResolver;
            if (inlineContentItemsProcessor != null) _mockInlineContentItemsProcessor = inlineContentItemsProcessor;
            var httpClient = httpMessageHandler != null ? httpMessageHandler.ToHttpClient() : MockHttp.ToHttpClient();

            var client = new DeliveryClient(
                DeliveryOptionsFactory.CreateMonitor(new DeliveryOptions { ProjectId = projectId.ToString() }),
                new ContentLinkResolver(_mockContentLinkUrlResolver),
                _mockInlineContentItemsProcessor,
                _mockModelProvider,
                _mockResiliencePolicyProvider,
                _mockTypeProvider,
                _mockPropertyMapper,
                new DeliveryHttpClient(httpClient)
            );

            return client;
        }

        internal static DeliveryClient GetMockedDeliveryClientWithOptions(DeliveryOptions options, MockHttpMessageHandler httpMessageHandler = null)
        {
            var deliveryHttpClient = new DeliveryHttpClient(httpMessageHandler != null ? httpMessageHandler.ToHttpClient() : MockHttp.ToHttpClient());
            var client = new DeliveryClient(
                DeliveryOptionsFactory.CreateMonitor(options),
                new ContentLinkResolver(_mockContentLinkUrlResolver),
                _mockInlineContentItemsProcessor,
                _mockModelProvider,
                _mockResiliencePolicyProvider,
                _mockTypeProvider,
                _mockPropertyMapper,
                deliveryHttpClient
            );

            return client;
        }
    }
}
