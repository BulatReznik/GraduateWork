﻿using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace YandexTrackerApi.BusinessLogic.Models.DiagramModels
{
    public class DiagramCreateCommand : IRequest<ResponseModel<string>>
    {
        [Required(ErrorMessage = "Поле Name должно быть заполнено")]
        public string Name { get; set; } = null!;

        [Required(ErrorMessage = "Поле XMLDiagram должно быть заполнено")]
        public string XMLDiagram { get; set; }

        [Required(ErrorMessage = "Поле ProjectId должно быть заполнено")]
        public Guid ProjectId { get; set; }

        [JsonIgnore]
        [Required(ErrorMessage = "Поле UserId должно быть заполнено")]
        public Guid UserId { get; set; }
    }
}
