using API_BookingHotel.Modules.Invoice.InvoiceModels;
using API_BookingHotel.Modules.Invoice.InvoicePassengerControllers;
using API_BookingHotel.Modules.Invoice.MInvoiceServices;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UnitTestHotel
{
    public class InvoiceApiTest
    {
        [Theory]
        [InlineData("Sussess", 1)]
        public async Task GetInvoicePassenger_ExpectedCodeSussess_ExitProduct(string key, int index)
        {
            var start = DateTime.Now.AddDays(-7);
            var end = DateTime.Now;

            var mockInvoiceService = new Mock<IInvoiceServices>();    // mock

            mockInvoiceService
                .Setup(s => s.GetListInvoicePasseners(key, start, end, index))
                .ReturnsAsync(new PagedResult<InvoiceViewModel>
                {
                    TotalRecords = 1,
                    PageSize = 10,
                    PageIndex = 1,
                    Items = new List<InvoiceViewModel>
                    {
                       new InvoiceViewModel
                       {
                         InvoiceCode = "INV001"

                       },
                       new InvoiceViewModel
                       {
                         InvoiceCode = "INV001"

                       },
                       new InvoiceViewModel
                       {
                         InvoiceCode = "INV001"

                       },
                       new InvoiceViewModel
                       {
                         InvoiceCode = "INV001"

                       }

                    }
                });

            var controller = new MInvoiceController(mockInvoiceService.Object);

            var result = await controller.GetInvoicePassenger(key, start, end, index);

            var okResult = Assert.IsType<OkObjectResult>(result);

            var data = Assert.IsType<PagedResult<InvoiceViewModel>>(okResult.Value);

            Assert.Equal(4, data.Items.Count());
            Assert.NotNull(data.Items);
            Assert.True(data.Items.Any());

        }

    }
}
