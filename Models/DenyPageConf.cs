namespace DenyPageCustom.Models
{
    public class DenyPageConf
    {
        public string tg_target       { get; set; } = "";
        public bool   show_qr         { get; set; } = true;
        public int    qr_size         { get; set; } = 200;
        public string page_badge      { get; set; } = "";
        public string page_title      { get; set; } = "";
        public string page_subtitle   { get; set; } = "";
        public string step1_text      { get; set; } = "";
        public string step2_text      { get; set; } = "";
        public string hint_text       { get; set; } = "";
        public string footer_text     { get; set; } = "";
        public string qr_caption      { get; set; } = "";
        public string qr_subcaption   { get; set; } = "";
        public string tg_button_text       { get; set; } = "Открыть Telegram";
        public bool   show_password_button { get; set; } = true;
        public bool   show_cub_button      { get; set; } = false;
        public bool   show_please_wait     { get; set; } = false;
        public string instruction_url      { get; set; } = "";
        public string instruction_text     { get; set; } = "";
    }
}
