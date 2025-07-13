using System.Collections.Generic;
using WebApi.Models;

namespace WebApi.Services;

public class ProductService
{
    private readonly List<Product> products = [
        new Product { Id = 1, Name = "Ultrabook Laptop", Description = "A sleek 14-inch ultrabook with a 2.8K OLED display, Intel Core i7-1360P processor, 16GB LPDDR5 RAM, 512GB NVMe SSD, and Intel Iris Xe graphics. Features a backlit keyboard, Thunderbolt 4 ports, Wi-Fi 6E, and up to 12 hours of battery life. Perfect for professionals and students on the go, running Windows 11 Home." }, 
        new Product { Id = 2, Name = "5G Smartphone", Description = "A cutting-edge 6.8-inch 5G smartphone with a 120Hz AMOLED display, Qualcomm Snapdragon 8 Gen 3 processor, 12GB RAM, 256GB storage, and a 48MP quad-camera system. Includes IP68 water resistance, 5000mAh battery with 65W fast charging, and Android 15 with AI-enhanced features for photography and productivity." }, 
        new Product { Id = 3, Name = "Gaming Monitor", Description = "A 27-inch 4K gaming monitor with a 144Hz refresh rate, 1ms response time, and IPS panel for vibrant colors. Supports HDR10, NVIDIA G-Sync, and AMD FreeSync Premium. Equipped with HDMI 2.1, DisplayPort 1.4, and USB-C connectivity, ideal for immersive gaming and content creation." }, 
        new Product { Id = 4, Name = "Wireless Earbuds", Description = "True wireless earbuds with active noise cancellation, 10mm drivers, and up to 32 hours of battery life with the charging case. Features IPX6 water resistance, Bluetooth 5.3, and touch controls. Offers low-latency mode for gaming and crystal-clear calls, compatible with iOS and Android." }, 
        new Product { Id = 5, Name = "Smartwatch", Description = "A premium smartwatch with a 1.4-inch AMOLED display, heart rate monitoring, SpO2 tracking, and GPS. Supports 100+ sports modes, sleep tracking, and up to 14 days of battery life. Water-resistant up to 5ATM, it syncs with iOS and Android for notifications and fitness data." }, 
        new Product { Id = 6, Name = "Convertible Tablet", Description = "A 12.4-inch convertible tablet with a 2K touchscreen, Qualcomm Snapdragon 7c Gen 2 processor, 8GB RAM, and 256GB SSD. Includes a detachable keyboard and stylus, Wi-Fi 6, and up to 10 hours of battery life. Runs Windows 11, ideal for work, creativity, and entertainment." }, 
        new Product { Id = 7, Name = "Wireless Gaming Mouse", Description = "A high-precision wireless gaming mouse with a 26,000 DPI optical sensor, 11 programmable buttons, and RGB lighting. Features a 2.4GHz wireless connection, up to 70 hours of battery life, and customizable weight tuning. Compatible with Windows and macOS for gaming and productivity." },
        new Product { Id = 8, Name = "Smart Home Speaker", Description = "A compact smart home speaker with voice assistant integration, 360-degree sound, and dual-band Wi-Fi. Supports Bluetooth 5.0, multi-room audio, and smart home control for lights, thermostats, and more. Delivers rich bass and clear vocals, compatible with iOS and Android." }, 
        new Product { Id = 9, Name = "Action Camera", Description = "A 4K action camera with a 170-degree wide-angle lens, 12MP sensor, and electronic image stabilization. Waterproof up to 40 meters, it supports Wi-Fi, time-lapse, and slow-motion modes. Includes a 1350mAh battery for up to 90 minutes of recording, ideal for outdoor adventures." },
        new Product { Id = 10, Name = "Portable SSD", Description = "A 1TB portable SSD with USB-C 3.2 Gen 2 for up to 1050MB/s read and write speeds. Features a rugged, shock-resistant design with IP55 water and dust resistance. Compatible with Windows, macOS, and gaming consoles, perfect for fast data transfers and backups." }
        ];
    public List<Product> GetAllProducts() => this.products;
}
