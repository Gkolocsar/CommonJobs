﻿using CommonJobs.Domain.MyMenu;
using CommonJobs.Infrastructure.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CommonJobs.Mvc.UI.Controllers
{
    public class MyMenuController : CommonJobsController
    {
        private string DetectUser()
        {
            //TODO: remove hardcoded "CS\\"
            //TODO: move to an AuthorizeAttribute or something more elegant
            if (User != null && User.Identity != null && User.Identity.Name != null && User.Identity.Name.StartsWith("CS\\"))
            {
                return User.Identity.Name.Substring(3);
            }
            else
            {
                throw new ApplicationException("User cannot be detected");
            }
        }

        public ActionResult Index()
        {
            return Edit(DetectUser(), true);
        }
        
        [CommonJobsAuthorize(Roles = "Users,MenuManagers")]
        public ActionResult Edit(string id, bool ownMenu = false)
        {
            ScriptManager.RegisterGlobalJavascript(
                "ViewData",
                new
                {
                    now = DateTime.Now,
                    menuUrl = ownMenu ? Url.Action("OwnMenu") : Url.Action("EmployeeMenu", new { id = id })
                },
                500);
            return View("MyMenu");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ActionName("OwnMenu")]
        public JsonNetResult PostOwnMenu(EmployeeMenu employeeMenu)
        {
            return PostEmployeeMenu(DetectUser(), employeeMenu);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [ActionName("OwnMenu")]
        public JsonNetResult GetOwnMenu()
        {
            return GetEmployeeMenu(DetectUser());
        }


        [CommonJobsAuthorize(Roles = "Users,MenuManagers")]
        [AcceptVerbs(HttpVerbs.Post)]
        [ActionName("EmployeeMenu")]
        public JsonNetResult PostEmployeeMenu(string id, EmployeeMenu employeeMenu)
        {
            if (string.IsNullOrWhiteSpace(id) || employeeMenu.employeeId != id)
                throw new ArgumentException(string.Format("No se permite modificar el menú del usuario o los ids no coinciden ({0}, {1})", id, employeeMenu.employeeId));

            //TODO: guardar el menú del empleado

            return Json(employeeMenu);
        }

        [CommonJobsAuthorize(Roles = "Users,MenuManagers")]
        [AcceptVerbs(HttpVerbs.Get)]
        [ActionName("EmployeeMenu")]
        public JsonNetResult GetEmployeeMenu(string id)
        {
            //TODO: cargar el menú del empleado
            
            return Json(new EmployeeMenu()
            {
                employeeId = id,
                name = "Moschini, Andrés",
                defaultPlace = "place_larioja",
                choices = new List<EmployeeMenuItem>() {
                    new EmployeeMenuItem() { week = 0, day = 1, option = "menu_vegetariano" },
                    new EmployeeMenuItem() { week = 0, day = 3, option = "menu_vegetariano", place = "place_garay" },
                    new EmployeeMenuItem() { week = 0, day = 4, option = "menu_vegetariano" },
                    new EmployeeMenuItem() { week = 0, day = 5, option = "menu_vegetariano" },
                    new EmployeeMenuItem() { week = 1, day = 1, option = "menu_vegetariano" },
                    new EmployeeMenuItem() { week = 1, day = 3, option = "menu_vegetariano", place = "place_garay" },
                    new EmployeeMenuItem() { week = 1, day = 4, option = "menu_vegetariano" },
                    new EmployeeMenuItem() { week = 1, day = 5, option = "menu_vegetariano" },
                    new EmployeeMenuItem() { week = 2, day = 1, option = "menu_vegetariano" },
                    new EmployeeMenuItem() { week = 2, day = 3, option = "menu_vegetariano", place = "place_garay" },
                    new EmployeeMenuItem() { week = 2, day = 4, option = "menu_vegetariano" },
                    new EmployeeMenuItem() { week = 2, day = 5, option = "menu_light" },
                    new EmployeeMenuItem() { week = 3, day = 1, option = "menu_vegetariano" },
                    new EmployeeMenuItem() { week = 3, day = 3, option = "menu_vegetariano", place = "place_garay" },
                    new EmployeeMenuItem() { week = 3, day = 4, option = "menu_vegetariano" },
                    new EmployeeMenuItem() { week = 3, day = 5, option = "menu_light" },
                    new EmployeeMenuItem() { week = 4, day = 1, option = "menu_vegetariano" },
                    new EmployeeMenuItem() { week = 4, day = 3, option = "menu_vegetariano", place = "place_garay" },
                    new EmployeeMenuItem() { week = 4, day = 4, option = "menu_vegetariano" },
                    new EmployeeMenuItem() { week = 4, day = 5, option = "menu_light" }
                },
                overrides = new List<EmployeeMenuOverrideItem>() {
                    new EmployeeMenuOverrideItem() { date = new DateTime(2012, 12, 3), cancel = true, comment = "Voy a comer mucha torta el domingo, prefiero no almorzar" },
                    new EmployeeMenuOverrideItem() { date = new DateTime(2012, 12, 5), place = "place_larioja", comment = "Se suspende la reunión de los miercoles en Garay" },
                    new EmployeeMenuOverrideItem() { date = new DateTime(2012, 12, 6), option = "menu_comun", comment = "Por ser jueves non santo voy a comer carne" }
                }
            });
        }

        [CommonJobsAuthorize(Roles = "Users,MenuManagers")]
        public ActionResult Admin()
        {
            ScriptManager.RegisterGlobalJavascript(
                "ViewData",
                new
                {
                    now = DateTime.Now
                },
                500);
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ActionName("MenuDefinition")]
        [CommonJobsAuthorize(Roles = "Users,MenuManagers")]
        public JsonNetResult PostMenuDefinition(Menu menuDefinition)
        {
            return Json(menuDefinition);
        }

        [AcceptVerbs(HttpVerbs.Get)]
        [ActionName("MenuDefinition")]
        public JsonNetResult GetMenuDefinition()
        {
            return Json(new Menu()
            {
                title = "Menú primaveral",
                firstWeek = 0,
                weeksQuantity = 5,
                deadlineTime = "9:30",
                startDate = new DateTime(2012, 9, 21),
                endDate = new DateTime(2020, 1, 1),
                places = new List<Place>() 
                {
                    new Place() { key = "place_larioja", text = "La Rioja" },
                    new Place() { key = "place_garay", text = "Garay" }
                },
                options = new List<Option>() 
                {
                    new Option() { key = "menu_comun", text = "Común" },
                    new Option() { key = "menu_light", text = "Light" },
                    new Option() { key = "menu_vegetariano", text = "Vegetariano" }
                },
                foods = new List<MenuItem>() 
                {
                    new MenuItem() { week = 0, day = 1, option = "menu_comun", food = "Cuarto de pollo deshuesado al champignonge c/papas" },
                    new MenuItem() { week = 0, day = 1, option = "menu_light", food = "Wok de vegetales y atun al natural" },
                    new MenuItem() { week = 0, day = 1, option = "menu_vegetariano", food = "Cintas caseras de rucula c/wok de hongos  y salsa de soja" },
                    new MenuItem() { week = 0, day = 2, option = "menu_comun", food = "Albondigas a la portuguesa c/pure de papas" },
                    new MenuItem() { week = 0, day = 2, option = "menu_light", food = "Ensalada de rucula,champig frescos,remolacha y huevo" },
                    new MenuItem() { week = 0, day = 2, option = "menu_vegetariano", food = "Fajitas Mexicanas veg c/ batatas al horno" },
                    new MenuItem() { week = 0, day = 3, option = "menu_comun", food = "Ravioles de pollo,puerro y muzarella c/ salsa mixta" },
                    new MenuItem() { week = 0, day = 3, option = "menu_light", food = "Milanesas de soja c/puré de calabaza y zanahoria" },
                    new MenuItem() { week = 0, day = 3, option = "menu_vegetariano", food = "Brusquetas de verduras gratinadas,aceitunas,tomates y espinaca" },
                    new MenuItem() { week = 0, day = 4, option = "menu_comun", food = "Milanesa a la suiza c/papas y cebollas al horno" },
                    new MenuItem() { week = 0, day = 4, option = "menu_light", food = "Pechuga sin piel al puerro c/vegetales grillados" },
                    new MenuItem() { week = 0, day = 4, option = "menu_vegetariano", food = "Sorrentinos de calabaza y muzzarella c/salsa mixta" },
                    new MenuItem() { week = 0, day = 5, option = "menu_comun", food = "Filet de merluza al roquefort c/papas al natural" },
                    new MenuItem() { week = 0, day = 5, option = "menu_light", food = "Ñoquis de ricota,salsa de tomates frescos y albahaca" },
                    new MenuItem() { week = 0, day = 5, option = "menu_vegetariano", food = "Pizza individual de espinaca y muzzarella" },
                    new MenuItem() { week = 1, day = 1, option = "menu_comun", food = "Sorrentinos jamon y quso c/salsa scarparo" },
                    new MenuItem() { week = 1, day = 1, option = "menu_light", food = "Ensalada roja de tomate,remolacha,zanahoria,repollo colorado y atun al natural" },
                    new MenuItem() { week = 1, day = 1, option = "menu_vegetariano", food = "Milanesa de berenjena a la napolitana c/puré mixto" },
                    new MenuItem() { week = 1, day = 2, option = "menu_comun", food = "Cuarto de pollo al verdeo c/papas" },
                    new MenuItem() { week = 1, day = 2, option = "menu_light", food = "Brusquetas de verduras grilladas,tomates y albahaca en pan integral y semillas" },
                    new MenuItem() { week = 1, day = 2, option = "menu_vegetariano", food = "Wok de vegetales hongos,brotes,semillas y salsa de soja" },
                    new MenuItem() { week = 1, day = 3, option = "menu_comun", food = "Milanesa napolitana c/puré mixto" },
                    new MenuItem() { week = 1, day = 3, option = "menu_light", food = "Pollo a la mostaza c/arroz integral" },
                    new MenuItem() { week = 1, day = 3, option = "menu_vegetariano", food = "Ñoquis de espicas y papas c/ salsa de puerro y tomates frescos" },
                    new MenuItem() { week = 1, day = 4, option = "menu_comun", food = "Merluza horneada caprese c/puré de papas y espinacas" },
                    new MenuItem() { week = 1, day = 4, option = "menu_light", food = "Risotto Integral de Verduras y espinaca" },
                    new MenuItem() { week = 1, day = 4, option = "menu_vegetariano", food = "Ensalada de verdes,huevos duros,parmesano,croutons y aderezos Cesar" },
                    new MenuItem() { week = 1, day = 5, option = "menu_comun", food = "Fajitas Mixtas c/papas provenzal" },
                    new MenuItem() { week = 1, day = 5, option = "menu_light", food = "Ravioles de ricota en masa integral c/salsa de verduras y tomates frescos" },
                    new MenuItem() { week = 1, day = 5, option = "menu_vegetariano", food = "Calzone de muzzarella,tomates,albahaca y muzzarella" },
                    new MenuItem() { week = 2, day = 1, option = "menu_comun", food = "Canelones de ricota y verduras a la bolognesa" },
                    new MenuItem() { week = 2, day = 1, option = "menu_light", food = "Pollo al puerro acompañado c/verduras asadas" },
                    new MenuItem() { week = 2, day = 1, option = "menu_vegetariano", food = "Milanesas de soja a la napolitana c/puré mixto" },
                    new MenuItem() { week = 2, day = 2, option = "menu_comun", food = "Arroz c/calamares" },
                    new MenuItem() { week = 2, day = 2, option = "menu_light", food = "Ensalada de hojas verdes, champignones, verdeo, claras de huevo grilladas y semillas" },
                    new MenuItem() { week = 2, day = 2, option = "menu_vegetariano", food = "Ñoquis de papa y espinaca c/salsa roquefort" },
                    new MenuItem() { week = 2, day = 3, option = "menu_comun", food = "Muslo relleno c/ jamon y queso y verduras asadas" },
                    new MenuItem() { week = 2, day = 3, option = "menu_light", food = "Niños envueltos c/puré de calabaza y hierbas" },
                    new MenuItem() { week = 2, day = 3, option = "menu_vegetariano", food = "Calabaza asada c/queso gratinado,choclo y puerro" },
                    new MenuItem() { week = 2, day = 4, option = "menu_comun", food = "Pizza individual napolitana c/jamon y aceitunas" },
                    new MenuItem() { week = 2, day = 4, option = "menu_light", food = "Merluza horneada al limon y hierbas c/calabaza asada" },
                    new MenuItem() { week = 2, day = 4, option = "menu_vegetariano", food = "Ensalada de hojas verdes,tomates,muzzarella,aceitunas negras y verdeo" },
                    new MenuItem() { week = 2, day = 5, option = "menu_comun", food = "Carne asada al horno con papas a las hierbas" },
                    new MenuItem() { week = 2, day = 5, option = "menu_light", food = "Wok de vegetales, brotes, hongos y semillas." },
                    new MenuItem() { week = 2, day = 5, option = "menu_vegetariano", food = "Ravioles de espinaca y parmesano c/salsa rosa" },
                    new MenuItem() { week = 3, day = 1, option = "menu_comun", food = "Pollo al verdeo c/calabaza asada" },
                    new MenuItem() { week = 3, day = 1, option = "menu_light", food = "Roulete integral de vegetales,brotes y semillas" },
                    new MenuItem() { week = 3, day = 1, option = "menu_vegetariano", food = "Milanesa de zapallitos c/muzzarella.tomate y albahaca" },
                    new MenuItem() { week = 3, day = 2, option = "menu_comun", food = "Lasagna a la bolognesa" },
                    new MenuItem() { week = 3, day = 2, option = "menu_light", food = "Ensalada de verdes c/pollo y citronete" },
                    new MenuItem() { week = 3, day = 2, option = "menu_vegetariano", food = "Pizza individual de muzzarella y vegetales gratinados c/pan casero" },
                    new MenuItem() { week = 3, day = 3, option = "menu_comun", food = "Fajitas mexicanas de ternera c/papas y batatas" },
                    new MenuItem() { week = 3, day = 3, option = "menu_light", food = "Ensalada de hojas verdes, atún al natural tomates y zanahoria" },
                    new MenuItem() { week = 3, day = 3, option = "menu_vegetariano", food = "Sorrentinos de vegetales grillados y ricota c/salsa de puerro y tomates" },
                    new MenuItem() { week = 3, day = 4, option = "menu_comun", food = "Suprema napolitana c/puré de papas y calabaza" },
                    new MenuItem() { week = 3, day = 4, option = "menu_light", food = "Pan de carne y vegetales al wok" },
                    new MenuItem() { week = 3, day = 4, option = "menu_vegetariano", food = "Ensalada caprese c/aceitunas negras y rucula" },
                    new MenuItem() { week = 3, day = 5, option = "menu_comun", food = "Matambre a la Pizza con Papas al orégano" },
                    new MenuItem() { week = 3, day = 5, option = "menu_light", food = "Omelete de claras,espinacas y calabaza asada c/pan integral" },
                    new MenuItem() { week = 3, day = 5, option = "menu_vegetariano", food = "Ñoquis de calabaza c/salsa de tomates y espinacas" },
                    new MenuItem() { week = 4, day = 1, option = "menu_comun", food = "Calzon especial c/jamon,morrones y muzzarella" },
                    new MenuItem() { week = 4, day = 1, option = "menu_light", food = "Wok de vegetales,pollo y semillas" },
                    new MenuItem() { week = 4, day = 1, option = "menu_vegetariano", food = "Milhojas de verdura,queso y muzzarella" },
                    new MenuItem() { week = 4, day = 2, option = "menu_comun", food = "Pastel de carne y papas con queso gratinado" },
                    new MenuItem() { week = 4, day = 2, option = "menu_light", food = "Pesca del día horneada al limon y hierbas c/calabaza asada" },
                    new MenuItem() { week = 4, day = 2, option = "menu_vegetariano", food = "Sorrentinos Caprese c/salsa de quesos" },
                    new MenuItem() { week = 4, day = 3, option = "menu_comun", food = "Milanesa con arroz a la crema" },
                    new MenuItem() { week = 4, day = 3, option = "menu_light", food = "Ensalada de espinacas frescas,champignones,semillas,verdeo y claras de huevo grilladas" },
                    new MenuItem() { week = 4, day = 3, option = "menu_vegetariano", food = "Pizza individual de muzzarella,rucula y parmesano y aceitunas negras" },
                    new MenuItem() { week = 4, day = 4, option = "menu_comun", food = "Fajitas mexicanas de pollo c/papas a la hierba" },
                    new MenuItem() { week = 4, day = 4, option = "menu_light", food = "Pollo al puerro c/verduras asadas" },
                    new MenuItem() { week = 4, day = 4, option = "menu_vegetariano", food = "Quesadillas mexicanas,c/queso y cebolla" },
                    new MenuItem() { week = 4, day = 5, option = "menu_comun", food = "Suprema a la milanesa c/puré de papas" },
                    new MenuItem() { week = 4, day = 5, option = "menu_light", food = "Pizza integral,c/verduras grilladas y salsa de tomates frescos" },
                    new MenuItem() { week = 4, day = 5, option = "menu_vegetariano", food = "Lasagna de ricota,vegetales,espinacas y salsa mixta gratinada" }
                }
            });
        }
    }
}
