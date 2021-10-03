using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Discount.Grpc.Services
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _repository;
        private readonly ILogger _logger;
        private readonly IMapper _mapper;

        public DiscountService(IDiscountRepository repository, ILogger logger, IMapper mapper)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper)); ;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            var coupon = await _repository.GetDiscount(request.ProductName);
            if(coupon == null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={request.ProductName} not exists.")); 
            }

            _logger.LogInformation("Discount is retrieved for ProductName : {productName} , Amount : {amount}", coupon.ProductName, coupon.Amount);
            var couponModel = _mapper.Map<CouponModel>(coupon);

            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            var mappedCoupon = _mapper.Map<Coupon>(request.Coupon);
            var coupon = await _repository.CreateDiscount(mappedCoupon);
            _logger.LogInformation("Discount is succsessfuly created. Productname : {ProductName}", mappedCoupon.ProductName);

            return _mapper.Map<CouponModel>(coupon);
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            var mappedCoupon = _mapper.Map<Coupon>(request.Coupon);
            var coupon = await _repository.UpdateDiscount(mappedCoupon);
            _logger.LogInformation("Discount is succsessfuly updated. Productname : {ProductName}", mappedCoupon.ProductName);

            return _mapper.Map<CouponModel>(coupon);
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            var deleted = await _repository.DeleteDiscount(request.ProductName);
            _logger.LogInformation("Discount is succsessfuly deleted. Productname : {ProductName}", request.ProductName);

            var response = new DeleteDiscountResponse
            {
                Success = deleted
            };

            return response;
        }
    }
}
