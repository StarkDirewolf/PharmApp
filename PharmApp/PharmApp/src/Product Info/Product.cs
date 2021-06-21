using PharmApp.src.Product_Info;
using PharmApp.src.SQL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PharmApp.src
{
    class Product
    {
        public string pipcode = "0";
        public string description = "NO DESCRIPTION";
        public string genericID = "0";
        public decimal unitsPerPack = 1.0M;
        //public string preparationCode;
        public bool isGeneric = false;
        public string supplier = "NO SUPPLIER";
        private OCRResult ocrResult;
        private SQLOrderHistory orders;
        public decimal dtPrice = 0.0M;
        private SQLOrderingNote _orderingNote;
        public OrderingNote orderingNote
        {
            get
            {
                if (_orderingNote == null)
                {
                    _orderingNote = new SQLOrderingNote(pipcode);
                }
                return _orderingNote.GetData();
            }
            set
            {
                if (_orderingNote == null) _orderingNote = new SQLOrderingNote(pipcode);

                if (_orderingNote.IsEmpty() && value.note != "")
                {
                    _orderingNote.SetData(value);

                    SQLQueryer.SaveOrderingNote(pipcode, value); ;
                }
                else if (!_orderingNote.NoteEquals(value.note) || !_orderingNote.RequiresActionEquals(value.requiresAction))
                {
                    _orderingNote.SetData(value);

                    if (value.note == "")
                    {
                        _orderingNote.Clear();
                        SQLQueryer.DeleteOrderingNote(pipcode);
                    }
                    else 
                    { 
                        
                        SQLQueryer.SaveOrderingNote(pipcode, value);
                    }
                    
                }
            }
        }
        private SQLOrderPadQuantities currentlyOnOrder;

        public OrderPadQuantities GetCurrentOrders()
        {
            if (currentlyOnOrder == null)
            {
                currentlyOnOrder = new SQLOrderPadQuantities(pipcode);
            }
            return currentlyOnOrder.GetData();
        }

        public OrderHistory GetPlacedOrders(DateTime fromDate, DateTime endDate)
        {
            if (orders == null || orders.fromDate != fromDate || orders.endDate != endDate)
            {
                if (isGeneric)
                {
                    orders = new SQLOrderHistory(genericID, fromDate, endDate, true);
                }
                else
                {
                    orders = new SQLOrderHistory(pipcode, fromDate, endDate, false);
                }
            }

            return orders.GetData();
        }

        public bool DeliveryDue()
        {
            OrderHistory history = GetRecentOrders();
            return history.DeliveryDue();
        }

        public bool IsOrdering()
        {
            OrderHistory history = GetRecentOrders();
            return history.IsOrdering();
        }

        public OrderHistory GetRecentOrders()
        {
            return GetPlacedOrders(DateTime.Today.AddDays(-5), DateTime.Today);
        }

        public void SetOCRResult(OCRResult ocr)
        {
            ocrResult = ocr;
        }

        public OCRResult GetOCRResult()
        {
            return ocrResult;
        }

        public override string ToString()
        {
            return description + " (" + Util.TrimTrailingZeros(unitsPerPack.ToString()) + ") - £" + Util.TrimTrailingZeros(dtPrice.ToString());
        }
    }
}
