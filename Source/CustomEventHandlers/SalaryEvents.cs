using Core.Common;
using CustomEventArgs;
using Data.Models;
using Model.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DocumentEvents;
using SalaryDocumentEvents;
using System.Data;

namespace Jobs.Controllers
{


    public class SalaryEvents : SalaryDocEvents
    {
        //For Subscribing Events
        public SalaryEvents()
        {
            Initialized = true;
            _onHeaderSave += SalaryEvents__onHeaderSave;
            _onLineSave += SalaryEvents__onLineSave;
            _onLineSaveBulk += SalaryEvents__onLineSaveBulk;
            _onLineDelete += SalaryEvents__onLineDelete;
            _onHeaderDelete += SalaryEvents__onHeaderDelete;
            _onHeaderSubmit += SalaryEvents__onHeaderSubmit;
            _beforeLineSave += SalaryEvents__beforeLineSaveDataValidation;
            _beforeLineDelete += SalaryEvents__beforeLineDeleteDataValidation;
            _beforeLineSaveBulk += SalaryEvents__beforeLineSaveBylkDataValidation;

        }


        void SalaryEvents__onHeaderSubmit(object sender, JobEventArgs EventArgs, ref ApplicationDbContext db)
        {
        }

        bool SalaryEvents__beforeLineSaveDataValidation(object sender, JobEventArgs EventArgs, ref ApplicationDbContext db)
        {


            return true;
        }

        bool SalaryEvents__beforeLineDeleteDataValidation(object sender, JobEventArgs EventArgs, ref ApplicationDbContext db)
        {


            return true;
        }


        bool SalaryEvents__beforeLineSaveBylkDataValidation(object sender, JobEventArgs EventArgs, ref ApplicationDbContext db)
        {



            return true;
        }


        void SalaryEvents__onHeaderDelete(object sender, JobEventArgs EventArgs, ref ApplicationDbContext db)
        {

        }

        void SalaryEvents__onLineDelete(object sender, JobEventArgs EventArgs, ref ApplicationDbContext db)
        {

        }

        void SalaryEvents__onLineSaveBulk(object sender, JobEventArgs EventArgs, ref ApplicationDbContext db)
        { 
        }

        void SalaryEvents__onLineSave(object sender, JobEventArgs EventArgs, ref ApplicationDbContext db)
        {
        }


        void SalaryEvents__onHeaderSave(object sender, JobEventArgs EventArgs, ref ApplicationDbContext db)
        {

        }

    }
}
