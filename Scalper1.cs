
#region Using declarations
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml.Serialization;
using NinjaTrader.Cbi;
using NinjaTrader.Gui;
using NinjaTrader.Gui.Chart;
using NinjaTrader.Gui.SuperDom;
using NinjaTrader.Gui.Tools;
using NinjaTrader.Data;
using NinjaTrader.NinjaScript;
using NinjaTrader.Core.FloatingPoint;
using NinjaTrader.NinjaScript.Indicators;
using NinjaTrader.NinjaScript.DrawingTools;
#endregion

//This namespace holds Strategies in this folder and is required. Do not change it. 
namespace NinjaTrader.NinjaScript.Strategies.EhsanStrats
{
	public class Scalper1 : Strategy
	{
		bool CanIGoLong	 		 = false;
		bool CanIGoShort		 = false;
		bool highChoppiness	 	 = false;
		bool upwardMom    	 	 = false;
		bool downwardMom    	 = false;
		bool highStrengthLong    = false;
		bool highStrengthShort   = false;
		bool highVolatility  	 = false;
		bool highVol		 	 = false;
		double sessionHigh		 = 0;
		double sessionLow		 = 0;
		int numTrade			 = 0;
		protected override void OnStateChange()
		{
			if (State == State.SetDefaults)
			{
				Description									= @"Scalper1";
				Name										= "Scalper1";
				Calculate									= Calculate.OnBarClose;
				EntriesPerDirection							= 1;
				EntryHandling								= EntryHandling.AllEntries;
				IsExitOnSessionCloseStrategy				= true;
				ExitOnSessionCloseSeconds					= 30;
				IsFillLimitOnTouch							= false;
				MaximumBarsLookBack							= MaximumBarsLookBack.TwoHundredFiftySix;
				OrderFillResolution							= OrderFillResolution.Standard;
				Slippage									= 2;
				StartBehavior								= StartBehavior.WaitUntilFlat;
				TimeInForce									= TimeInForce.Gtc;
				TraceOrders									= false;
				RealtimeErrorHandling						= RealtimeErrorHandling.StopCancelClose;
				StopTargetHandling							= StopTargetHandling.PerEntryExecution;
				BarsRequiredToTrade							= 20;IncludeCommission = true;
				IncludeCommission = true;
				// Disable this property for performance gains in Strategy Analyzer optimizations
				// See the Help Guide for additional information
				IsInstantiatedOnEachOptimizationIteration	= true;
			//Indicator setting
				
				
				StarttimeScalper1 = DateTime.Parse("18:00", System.Globalization.CultureInfo.InvariantCulture);
           		EndtimeScalper1 = DateTime.Parse("16:00", System.Globalization.CultureInfo.InvariantCulture);
				qtyScalper1L											= 1;
				qtyScalper1S											= 1;
				
			// Strategy Settings
                LongPTScalper1										= 100;
                LongSLScalper1	 									= 100;
                ShortPTScalper1	 									= 100;
                ShortSLScalper1										= 100;
				SignalNameScalper1L 									= "Scalper1Long";
    			SignalNameScalper1S 									= "Scalper1Short";
				numBar1													= 5;
				numBar2													= 30;
				numBar3													= 0;
				numBar4													= 1;
				smaPeriod = 60;
				atrPeriod = 20;
				smaOffset = 0.1;
				atrFactor = 1.5;
			// Filter Settings
				ADXValLong												= 0;
				ADXValShort												= 0;
				ADXLen													= 20;
				ADXValSlopeValLong											= -0.1;
				ADXValSlopeValShort											= 0.5;
				ChoppinessVal											= 10;
				ChoppinessLen											= 14;
				RSILen													= 15;
				RSIValShort												= 30;
				RSIValLong												= 65;
				VOLSmallLen												= 1;
				VOLLargeLen												= 5;
				ATRSmallLen												= 1;
				ATRLargeLen												= 70;
				SMALen													= 60;
				EMALen													= 100;
				KCLen													= 30;
				KCStdDev												= 2;
				canLen0													= 1;
				canLen1													= 4;
				canLen2													= 3;
			// Multi-time frame setting
				Ticker1													= "NQ 03-24";
				Timeframe1				 								= 1;
				
			// Drawing
				AddPlot(Brushes.Green,"upperChannel"); //stored as Plots[0] and Values[0]
				AddPlot(Brushes.Blue,"lowerChannel"); //stored as Plots[1] and Values[1]
				

			}
			else if (State == State.Configure)
			{
				//MNQTrenFollower
				AddDataSeries(Ticker1, Data.BarsPeriodType.Minute, Timeframe1);
				SetProfitTarget(CalculationMode.Ticks, LongPTScalper1);
				SetStopLoss(CalculationMode.Ticks, LongSLScalper1);
				//SetProfitTarget(CalculationMode.Ticks, ShortPTScalper1);
				//SetStopLoss(CalculationMode.Ticks, ShortSLScalper1);
				
				
			}
			else if (State == State.DataLoaded)
			{
				//AddChartIndicator(SMA(BarsArray[0],numBar1));
				AddChartIndicator(VOL());
				//AddChartIndicator(SMA(BarsArray[0],numBar2));
				//ChartIndicators[0].Plots[0].Brush = Brushes.Black;
				//ChartIndicators[1].Plots[0].Brush = Brushes.Blue;
			}
		}
		protected override void OnBarUpdate()
	    {
			if (CurrentBars[0] < 100) return;
			if (CurrentBars[1] < 100) return;
			DateTime currentTime = Times[0][0];
			double r1Value = Pivots(PivotRange.Daily, HLCCalculationMode.CalcFromIntradayData, 0, 0, 0, 20).R1[0];
            double s1Value = Pivots(PivotRange.Daily, HLCCalculationMode.CalcFromIntradayData, 0, 0, 0, 20).S1[0];
			//Initialization
			CanIGoLong	 	 = false;
			CanIGoShort	     = false;
			upwardMom    	 = false;
			downwardMom      = false;
			highChoppiness   = false;
			highStrengthLong     = false;
			highStrengthShort     = false;
			highVolatility   = false;
			highVol		     = false;
			// upward Direction
			if ( 
				//Close[0] > SMA(SMALen)[0] //&&
				//Closes[1][0] > EMA(Closes[1],EMALen)[0] &&
				Slope(EMA(BarsArray[1],EMALen), 5, 0) > 0 &&
				RSI(BarsArray[1],RSILen,3)[0] < RSIValLong
				//CrossAbove(Close, KeltnerChannel(KCStdDev,KCLen).Upper[0],1) && 
				//CrossAbove(Close, SMA(SMALen),numBar4)
				)
			{
				upwardMom = true;
			}
			// downward Direction
			if ( 
				//Close[0] < SMA(SMALen)[0] //&&
				//Closes[1][0] < EMA(Closes[1],EMALen)[0] &&
				Slope(EMA(BarsArray[1],EMALen), 5, 0) < 0 &&
				RSI(BarsArray[1],RSILen,3)[0] > RSIValShort
				//CrossBelow(Close, KeltnerChannel(KCStdDev,KCLen).Lower[0],1) && 
				//CrossBelow(Close, SMA(SMALen),numBar4)
				)
			{
				downwardMom = true;
			}
			// Strength Long
			if  ( 
				ADX(BarsArray[1],ADXLen)[0] > ADXValLong &&
				Slope(ADX(BarsArray[1],ADXLen), 5, 0) > ADXValSlopeValLong
				)
			{
				highStrengthLong = true;
			}
			// Strength Short
			if  ( 
				ADX(BarsArray[1],ADXLen)[0] > ADXValShort &&
				Slope(ADX(BarsArray[1],ADXLen), 5, 0) > ADXValSlopeValShort
				)
			{
				highStrengthShort = true;
			}
			//Choppiness
			if  ( 
				ChoppinessIndex(BarsArray[1] , ChoppinessLen)[0] > ChoppinessVal
				)
			{
				highChoppiness = true;
			}
			
			// Volatility
			if  ( 
				ATR(BarsArray[1],ATRSmallLen)[0] > ATR(BarsArray[1],ATRLargeLen)[0]
				)
			{
				highVolatility = true;
			}
			// Volume
			if  ( 
				SMA(VOL(BarsArray[1]),VOLSmallLen)[0] > SMA(VOL(BarsArray[1]),VOLLargeLen)[1]
				)
			{
				highVol = true;
			}
			
			// Can I go Long
			
			if  ( 
				upwardMom &&
				highStrengthLong //&& 
				//highVolatility //&& 
				//highVol //&&
				//highChoppiness
				)
			{
				CanIGoLong = true;
			}
			// Can I go Short
			
			if  ( 
				downwardMom &&
				highStrengthShort //&&
				///highVolatility //&& 
				//highVol //&&
				//highChoppiness
				)
			{
				CanIGoShort = true;
			}
			
			if (currentTime.TimeOfDay >= EndtimeScalper1.TimeOfDay && Position.MarketPosition == MarketPosition.Long) ExitLong();
			if (currentTime.TimeOfDay >= EndtimeScalper1.TimeOfDay && Position.MarketPosition == MarketPosition.Short) ExitShort();			
			
			if (BarsInProgress == 0)
			{
			
				if (EndtimeScalper1.TimeOfDay < StarttimeScalper1.TimeOfDay)
	        	{
		            if (currentTime.TimeOfDay >= StarttimeScalper1.TimeOfDay || currentTime.TimeOfDay < EndtimeScalper1.TimeOfDay)
			            {
							 if ( Close[1]>Open[1] &&
								 Close[2]<Open[2] &&
								 Close[1] >= Open[2] &&
								 Open[1] <= Close[2] &&
								 Close[0] > Open[0] &&
								 High[0]-Close[0] < Close[0]-Open[0] &&
								 Open[2] - Close[2] > canLen2 &&
								 Close[1] - Open[1] > canLen1 &&
								 Close[0] - Open[0] > canLen0 &&
								 //Low[0] > Open[1] &&
								 //Close[4] > Open[4]  &&
								 CanIGoLong )
								{
			                    EnterLong(qtyScalper1L, SignalNameScalper1L);
		                        double stopLossPrice = Close[0] - LongSLScalper1 * TickSize;
		                        SetStopLoss(SignalNameScalper1L, CalculationMode.Price, stopLossPrice, false);

		                        double profitTargetPrice = Close[0] + LongPTScalper1 * TickSize;
		                        SetProfitTarget(SignalNameScalper1L, CalculationMode.Price, profitTargetPrice);
			                	}

							if (Close[1]<Open[1] &&
								 Close[2]>Open[2] &&
								 Close[1] <= Open[2] &&
								 Open[1] >= Close[2] &&
								 Close[0] < Open[0] &&
								 Close[0]-Low[0] < Open[0]-Close[0] &&
								 Close[2] - Open[2] > canLen2 &&
								 Open[1] - Close[1] > canLen1 &&
								 Open[0] - Close[0] > canLen0 &&
								 //High[0] < Open[1] &&
								 //Close[4] < Open[4]  &&
								 CanIGoShort)
								{
			                    EnterShort(qtyScalper1S, SignalNameScalper1S);
		                        double stopLossPrice = Close[0] + ShortSLScalper1 * TickSize;
		                        SetStopLoss(SignalNameScalper1S, CalculationMode.Price, stopLossPrice, false);

		                        double profitTargetPrice = Close[0] - ShortPTScalper1 * TickSize;
		                        SetProfitTarget(SignalNameScalper1S, CalculationMode.Price, profitTargetPrice);
			                	}			
			            }
			        }
			        else
			        {
			            if (currentTime.TimeOfDay >= StarttimeScalper1.TimeOfDay && currentTime.TimeOfDay < EndtimeScalper1.TimeOfDay)
			            {
							if ( Close[1]>Open[1] &&
								 Close[2]<Open[2] &&
								 Close[1] >= Open[2] &&
								 Open[1] <= Close[2] &&
								 Close[0] > Open[0] &&
								 High[0]-Close[0] < Close[0]-Open[0] &&
								 Open[2] - Close[2] > canLen2 &&
								 Close[1] - Open[1] > canLen1 &&
								 Close[0] - Open[0] > canLen0 &&
								 //Low[0] > Open[1] &&
								 //Close[4] > Open[4]  &&
								 CanIGoLong )
								{
			                    EnterLong(qtyScalper1L, SignalNameScalper1L);
		                        double stopLossPrice = Close[0] - LongSLScalper1 * TickSize;
		                        SetStopLoss(SignalNameScalper1L, CalculationMode.Price, stopLossPrice, false);

		                        double profitTargetPrice = Close[0] + LongPTScalper1 * TickSize;
		                        SetProfitTarget(SignalNameScalper1L, CalculationMode.Price, profitTargetPrice);
			                	}

							if (Close[1]<Open[1] &&
								 Close[2]>Open[2] &&
								 Close[1] <= Open[2] &&
								 Open[1] >= Close[2] &&
								 Close[0] < Open[0] &&
								 Close[0]-Low[0] < Open[0]-Close[0] &&
								 Close[2] - Open[2] > canLen2 &&
								 Open[1] - Close[1] > canLen1 &&
								 Open[0] - Close[0] > canLen0 &&
								 //High[0] < Open[1] &&
								 //Close[4] < Open[4]  &&
								 CanIGoShort)
								{
			                    EnterShort(qtyScalper1S, SignalNameScalper1S);
		                        double stopLossPrice = Close[0] + ShortSLScalper1 * TickSize;
		                        SetStopLoss(SignalNameScalper1S, CalculationMode.Price, stopLossPrice, false);

		                        double profitTargetPrice = Close[0] - ShortPTScalper1 * TickSize;
		                        SetProfitTarget(SignalNameScalper1S, CalculationMode.Price, profitTargetPrice);
			                	}		
			            }
			        }
			}
		}

		
		#region Properties
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "KC Length",  Order = 0,  GroupName = "Filter settings")]
		public int KCLen
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "KC Std Dev",  Order = 0,  GroupName = "Filter settings")]
		public double KCStdDev
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "candle length 0",  Order = 0,  GroupName = "Filter settings")]
		public double canLen0
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "candle length 1",  Order = 0,  GroupName = "Filter settings")]
		public double canLen1
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "candle length 2",  Order = 0,  GroupName = "Filter settings")]
		public double canLen2
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Choppiness  Threshold",  Order = 0,  GroupName = "Filter settings")]
		public double ChoppinessVal
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "ADX Threshold Long",  Order = 0,  GroupName = "Filter settings")]
		public double ADXValLong
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "ADX Threshold Short",  Order = 0,  GroupName = "Filter settings")]
		public double ADXValShort
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "ADX Slope Threshold Short",  Order = 0,  GroupName = "Filter settings")]
		public double ADXValSlopeValShort
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "ADX Slope Threshold Long",  Order = 0,  GroupName = "Filter settings")]
		public double ADXValSlopeValLong
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "ADX Length",  Order = 0,  GroupName = "Filter settings")]
		public int ADXLen
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "RSI Threshold Short",  Order = 0,  GroupName = "Filter settings")]
		public double RSIValShort
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "RSI Threshold Long",  Order = 0,  GroupName = "Filter settings")]
		public double RSIValLong
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "RSI Length",  Order = 0,  GroupName = "Filter settings")]
		public int RSILen
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Choppiness Length",  Order = 0,  GroupName = "Filter settings")]
		public int ChoppinessLen
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "SMA Length",  Order = 0,  GroupName = "Filter settings")]
		public int SMALen
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "EMA Length",  Order = 0,  GroupName = "Filter settings")]
		public int EMALen
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "VOL Length Small",  Order = 0,  GroupName = "Filter settings")]
		public int VOLSmallLen
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "VOL Length Large",  Order = 0,  GroupName = "Filter settings")]
		public int VOLLargeLen
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "ATR Length Small",  Order = 0,  GroupName = "Filter settings")]
		public int ATRSmallLen
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "ATR Length Large",  Order = 0,  GroupName = "Filter settings")]
		public int ATRLargeLen
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "SMA Period",  Order = 0,  GroupName = "Strategy settings")]
		public int smaPeriod
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "ATR Period",  Order = 0,  GroupName = "Strategy settings")]
		public int atrPeriod
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "ATR factor",  Order = 0,  GroupName = "Strategy settings")]
		public double atrFactor
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "SMA Offset",  Order = 0,  GroupName = "Strategy settings")]
		public double smaOffset
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Look back number 1",  Order = 0,  GroupName = "Strategy settings")]
		public int numBar1
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Look back number 2",  Order = 0,  GroupName = "Strategy settings")]
		public int numBar2
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Look back number 3",  Order = 0,  GroupName = "Strategy settings")]
		public int numBar3
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(ResourceType = typeof(Custom.Resource), Name = "Look back number 4",  Order = 0,  GroupName = "Strategy settings")]
		public int numBar4
		{get; set;}
		
		[NinjaScriptProperty]
		[Display(Name = "Signal Name Scalper1 Long", Description = "Signal name for Scalper1 Long entry", Order = 400, GroupName = "xName Settings")]
		public string SignalNameScalper1L { get; set; }

		[NinjaScriptProperty]
		[Display(Name = "Signal Name Scalper1 Short", Description = "Signal name for Scalper1 Short entry", Order = 400, GroupName = "xName Settings")]
		public string SignalNameScalper1S { get; set; }
		
		[NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Long Profit Tick Target", Order = 1,  GroupName = "Scalper1")]
        public double LongPTScalper1 { get; set; }

        [NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Long Stop Tick Loss", Order = 2,  GroupName = "Scalper1")]
        public double LongSLScalper1 { get; set; }
		
        [NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Short Profit Tick Target", Order = 3,  GroupName = "Scalper1")]
        public double ShortPTScalper1 { get; set; }

        [NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Short Stop Tick Loss", Order = 4,  GroupName = "Scalper1")]
        public double ShortSLScalper1 { get; set; }
	 	
        [NinjaScriptProperty]
        [PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
        [Display(Name = "Session Start 1", Description = "Trading Session Start Time", Order = 7,  GroupName = "Scalper1")]
        public DateTime StarttimeScalper1
        { get; set; }
 		
        [NinjaScriptProperty]
        [PropertyEditor("NinjaTrader.Gui.Tools.TimeEditorKey")]
        [Display(Name = "Session End 1", Description = "Trading Session End Time", Order = 8,  GroupName = "Scalper1")]
        public DateTime EndtimeScalper1
    	{ get; set; }
 		
	   
        [NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Long Contract Quantity", Order = 7, GroupName = "Scalper1")]
        public int qtyScalper1L
        { get; set; }

        [NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Short Contract Quantity", Order = 7, GroupName = "Scalper1")]
        public int qtyScalper1S
        { get; set; }
		
		[NinjaScriptProperty]
		[Display(Name = "Scalper1 higher time frame ticker", Description = "Higher time frame ticker for Scalper1", Order = 20, GroupName = "Scalper1 Settings")]
		public string Ticker1 { get; set; }
		
		[NinjaScriptProperty]
        [Display(ResourceType = typeof(Custom.Resource), Name = "Scalper1 higher time frame", Order = 21, GroupName = "Scalper1 Settings")]
        public int Timeframe1
        { get; set; }
		
		#endregion
	}
}
