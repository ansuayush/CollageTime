using ExecViewHrk.Domain.Interface;
using ExecViewHrk.Models;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using ExecViewHrk.EfClient;
using System;

namespace ExecViewHrk.Domain.Repositories
{
    public class HoursCodesRepository : RepositoryBase, IHoursCodesRepository
    {
        #region List and Details. Insert, Update and Delete

        /// <summary>
        /// Returns the List of HoursCodes records
        /// </summary>
        /// <returns></returns>

        public List<HoursCodeVm> HoursCodesList_Read()
        {
            List<HoursCodeVm> getHourCodesList = new List<HoursCodeVm>();
            try
            {
                getHourCodesList = _context.HoursCodes
                     .Include(x => x.HoursCodeCode)
                    .Select(h => new HoursCodeVm
                    {
                        HoursCodeId = h.HoursCodeId,
                        HoursCodeDescription = h.HoursCodeDescription,
                        HoursCodeCode = h.HoursCodeCode,
                        IsHoursCodeActive = h.IsHoursCodeActive,
                        RateMultiplier = h.RateMultiplier,
                        RateOverride = h.RateOverride,
                        SubtractFromRegular = h.SubtractFromRegular,
                        NonPayCode = h.NonPayCode,
                        ADPAccNumberId = h.ADPAccNumberId,
                        ADPFieldMappingId = h.ADPFieldMappingId,
                        ExcludeFromOT = h.ExcludeFromOT,
                        CompanyCodeId = h.CompanyCodeId,
                        StartDate=h.StartDate,
                        EndDate=h.EndDate
                    }).Distinct().OrderBy(s => s.HoursCodeId).ToList();
                return getHourCodesList;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Returns the Details by HoursCodeId
        /// </summary>
        /// <param name="HoursCodeId"></param>
        /// <returns></returns>

        public HoursCodeVm HoursEditMatrix(int HoursCodeId)
        {
            HoursCodeVm hoursCodeVm = new HoursCodeVm();
            try
            {
                if (HoursCodeId != 0)
                {
                    hoursCodeVm = _context.HoursCodes.Where(h => h.HoursCodeId == HoursCodeId).Select(h => new HoursCodeVm
                    {
                        HoursCodeId = h.HoursCodeId,
                        HoursCodeDescription = h.HoursCodeDescription,
                        HoursCodeCode = h.HoursCodeCode,
                        IsHoursCodeActive = h.IsHoursCodeActive,
                        IsRetro=h.IsRetro,
                        RateMultiplier = h.RateMultiplier,
                        RateOverride = h.RateOverride,
                        SubtractFromRegular = h.SubtractFromRegular,
                        NonPayCode = h.NonPayCode,
                        ADPAccNumberId = h.ADPAccNumberId,
                        ADPFieldMappingId = h.ADPFieldMappingId,
                        ExcludeFromOT = h.ExcludeFromOT,
                        CompanyCodeId = h.CompanyCodeId,
                        StartDate=h.StartDate,
                        EndDate=h.EndDate
                    }).FirstOrDefault();
                }

                return hoursCodeVm;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// Inserts and Updates the HoursCodes records. And Throws the Model validation for Duplication
        /// </summary>
        /// <param name="hourcodeVM"></param>
        /// <returns></returns>

        public bool HoursCodeSaveAjax(HoursCodeVm hourcodeVM)
        {
            bool result = false;
            var hourcodesDetails = _context.HoursCodes.Where(x => x.HoursCodeId == hourcodeVM.HoursCodeId).FirstOrDefault();
            if (hourcodesDetails != null)
            {
                hourcodesDetails.CompanyCodeId = hourcodeVM.CompanyCodeId;
                hourcodesDetails.HoursCodeCode = hourcodeVM.HoursCodeCode;
                hourcodesDetails.HoursCodeDescription = hourcodeVM.HoursCodeDescription;
                hourcodesDetails.ADPFieldMappingId = hourcodeVM.ADPFieldMappingId;
                hourcodesDetails.ADPAccNumberId = (hourcodeVM.ADPAccNumberId <= 0) ? null : hourcodeVM.ADPAccNumberId;
                hourcodesDetails.RateOverride = hourcodeVM.RateOverride;
                hourcodesDetails.RateMultiplier = hourcodeVM.RateMultiplier;
                hourcodesDetails.ExcludeFromOT = hourcodeVM.ExcludeFromOT;
                hourcodesDetails.SubtractFromRegular = hourcodeVM.SubtractFromRegular;
                hourcodesDetails.NonPayCode = hourcodeVM.NonPayCode;
                hourcodesDetails.IsHoursCodeActive = hourcodeVM.IsHoursCodeActive;
                hourcodesDetails.StartDate = hourcodeVM.StartDate;
                hourcodesDetails.EndDate = hourcodeVM.EndDate;
                hourcodesDetails.IsRetro = hourcodeVM.IsRetro = false ? false : hourcodeVM.IsRetro;
                _context.SaveChanges();
                result = true;
            }
            else
            {
                var newHoursCode = new HoursCode()
                {
                    CompanyCodeId = hourcodeVM.CompanyCodeId,
                    HoursCodeDescription = hourcodeVM.HoursCodeDescription,
                    HoursCodeCode = hourcodeVM.HoursCodeCode,
                    ADPFieldMappingId = hourcodeVM.ADPFieldMappingId,
                    ADPAccNumberId = (hourcodeVM.ADPAccNumberId <= 0) ? null : hourcodeVM.ADPAccNumberId,
                    RateOverride = hourcodeVM.RateOverride,
                    RateMultiplier = hourcodeVM.RateMultiplier,
                    ExcludeFromOT = hourcodeVM.ExcludeFromOT,
                    SubtractFromRegular = hourcodeVM.SubtractFromRegular,
                    NonPayCode = hourcodeVM.NonPayCode,
                    IsHoursCodeActive = true,
                    IsRetro = hourcodeVM.IsRetro,
                    StartDate = hourcodeVM.StartDate,
                    EndDate = hourcodeVM.EndDate
                };
                _context.HoursCodes.Add(newHoursCode);
                _context.SaveChanges();
                result = true;
            }
            return result;
        }

        /// <summary>
        /// Deletes the record.
        /// </summary>
        /// <param name="HoursCodeId"></param>
        /// <returns></returns>
        public bool HourCodeDestroy(int HoursCodeId)
        {
            DeleteTimeCards(HoursCodeId);
            DeleteRetrohour(HoursCodeId);
            var result = false;
            var hourcode = _context.HoursCodes.Where(x => x.HoursCodeId == HoursCodeId).SingleOrDefault();
            if (hourcode != null)
            {
                _context.HoursCodes.Remove(hourcode);
                _context.SaveChanges();
                result = true;
            }
            return result;
        }


        public bool DeleteRetrohour(int HoursCodeId)
        {
            bool result = false;
            try
            {
                var EmployeeRetroHoursInDb = _context.EmployeeRetroHours
                .Where(x => x.HoursCodeId == HoursCodeId).FirstOrDefault();
                if (EmployeeRetroHoursInDb != null)
                {
                    _context.EmployeeRetroHours.Remove(EmployeeRetroHoursInDb);
                    _context.SaveChanges();
                }
            }
            catch (Exception err)
            {
                string message = err.Message;
                return result;
            }

            return result;

        }

        public bool DeleteTimeCards(int HoursCodeId)
        {
            bool result = false;
            try
            {
                TimeCard timeCardRecordInDb = _context.TimeCards
                    .Where(x => x.HoursCodeId == HoursCodeId).SingleOrDefault();
                if (timeCardRecordInDb != null)
                {                   
                    _context.TimeCards.Remove(timeCardRecordInDb);
                    result = _context.SaveChanges() > 0 ? true : false;
                }
            }
            catch (Exception err)
            {
                string message = err.Message;
                return result;
            }
            return result;
        }

        #endregion
    }
}
