using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace ExecViewHrk.WebUI.Helpers
{
    /// <summary>
    /// Stamps an optional signature onto the bottom of the last PDF page
    /// (does not add a new page when there is room on the existing page).
    /// </summary>
    public static class PdfSignatureHelper
    {
        /// <summary>
        /// Draws the signature in a box at the bottom of the last page of the PDF.
        /// </summary>
        public static byte[] AppendSignaturePage(byte[] pdfBytes, Stream signatureImageStream, string signerName = null)
        {
            if (pdfBytes == null || pdfBytes.Length == 0)
                throw new ArgumentException("PDF is empty.", "pdfBytes");
            if (signatureImageStream == null)
                throw new ArgumentNullException("signatureImageStream");

            using (var input = new MemoryStream(pdfBytes))
            using (var output = new MemoryStream())
            {
                PdfDocument document = PdfReader.Open(input, PdfDocumentOpenMode.Modify);
                if (document.PageCount == 0)
                    throw new InvalidOperationException("PDF has no pages.");

                PdfPage page = document.Pages[document.PageCount - 1];
                double pageW = page.Width.Point;
                double pageH = page.Height.Point;

                // Signature strip along the bottom of the last page (~1.35" tall)
                double margin = 28;
                double boxH = Math.Min(100, pageH * 0.18);
                double boxW = pageW - (margin * 2);
                double boxX = margin;
                // XGraphics uses top-left origin: bottom of page is near pageH
                double boxY = pageH - margin - boxH;

                signatureImageStream.Position = 0;
                using (Image gdiSig = Image.FromStream(signatureImageStream))
                using (XGraphics gfx = XGraphics.FromPdfPage(page, XGraphicsPdfPageOptions.Append))
                using (XImage sigImage = XImage.FromGdiPlusImage(gdiSig))
                {
                    // White background so signature stays readable over any footer area
                    gfx.DrawRectangle(XBrushes.White, boxX - 2, boxY - 18, boxW + 4, boxH + 22);

                    XFont titleFont = new XFont("Arial", 9, XFontStyle.Bold);
                    XFont labelFont = new XFont("Arial", 8, XFontStyle.Regular);
                    gfx.DrawString("Signature", titleFont, XBrushes.DimGray, boxX, boxY - 4);

                    if (!string.IsNullOrWhiteSpace(signerName))
                    {
                        gfx.DrawString(
                            "Signed by: " + signerName.Trim(),
                            labelFont,
                            XBrushes.DimGray,
                            boxX + 70,
                            boxY - 4);
                    }

                    // Border
                    XPen border = new XPen(XColor.FromArgb(160, 160, 160), 0.8);
                    gfx.DrawRectangle(border, boxX, boxY, boxW, boxH);

                    // Fit signature image inside the box with padding
                    double pad = 8;
                    double maxW = boxW - pad * 2;
                    double maxH = boxH - pad * 2;
                    double scale = Math.Min(maxW / sigImage.PixelWidth, maxH / sigImage.PixelHeight);
                    if (scale > 2.5) scale = 2.5;
                    double drawW = sigImage.PixelWidth * scale;
                    double drawH = sigImage.PixelHeight * scale;
                    double drawX = boxX + (boxW - drawW) / 2.0;
                    double drawY = boxY + (boxH - drawH) / 2.0;

                    gfx.DrawImage(sigImage, drawX, drawY, drawW, drawH);
                }

                document.Save(output, false);
                return output.ToArray();
            }
        }

        /// <summary>
        /// Kept for callers that still build a standalone signature page PDF.
        /// Prefer <see cref="AppendSignaturePage"/> which stamps the last page.
        /// </summary>
        public static byte[] BuildSignaturePagePdf(Stream signatureImageStream, string signerName)
        {
            const int dpi = 150;
            int pageW = (int)(8.5 * dpi);
            int pageH = (int)(11 * dpi);

            using (var pageBmp = new Bitmap(pageW, pageH))
            using (var g = Graphics.FromImage(pageBmp))
            using (var sigImg = Image.FromStream(signatureImageStream))
            using (var jpegMs = new MemoryStream())
            {
                g.Clear(Color.White);
                g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
                g.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;

                using (var titleFont = new Font("Arial", 18, FontStyle.Bold))
                using (var labelFont = new Font("Arial", 12, FontStyle.Regular))
                using (var brush = new SolidBrush(Color.FromArgb(28, 42, 34)))
                {
                    g.DrawString("Document Signature", titleFont, brush, 60, 60);
                    if (!string.IsNullOrWhiteSpace(signerName))
                        g.DrawString("Signed by: " + signerName, labelFont, brush, 60, 110);
                }

                int boxX = 60;
                int boxY = pageH - 420;
                int boxW = pageW - 120;
                int boxH = 280;
                using (var pen = new Pen(Color.FromArgb(180, 180, 180), 2))
                    g.DrawRectangle(pen, boxX, boxY, boxW, boxH);

                float pad = 24f;
                float maxW = boxW - pad * 2;
                float maxH = boxH - pad * 2 - 20;
                float scale = Math.Min(maxW / sigImg.Width, maxH / sigImg.Height);
                float drawW = sigImg.Width * scale;
                float drawH = sigImg.Height * scale;
                float drawX = boxX + (boxW - drawW) / 2f;
                float drawY = boxY + 30 + (maxH - drawH) / 2f;
                g.DrawImage(sigImg, drawX, drawY, drawW, drawH);

                pageBmp.Save(jpegMs, ImageFormat.Jpeg);
                jpegMs.Position = 0;
                return SimplePdfBuilder.BuildPdfFromImageStreams(new Stream[] { jpegMs });
            }
        }

        public static byte[] ConcatenatePdfs(byte[] firstPdf, byte[] secondPdf)
        {
            using (var firstStream = new MemoryStream(firstPdf))
            using (var secondStream = new MemoryStream(secondPdf))
            using (var output = new MemoryStream())
            {
                PdfDocument first = PdfReader.Open(firstStream, PdfDocumentOpenMode.Import);
                PdfDocument second = PdfReader.Open(secondStream, PdfDocumentOpenMode.Import);
                PdfDocument outputDoc = new PdfDocument();
                CopyPages(first, outputDoc);
                CopyPages(second, outputDoc);
                outputDoc.Save(output, false);
                return output.ToArray();
            }
        }

        private static void CopyPages(PdfDocument from, PdfDocument to)
        {
            for (int i = 0; i < from.PageCount; i++)
                to.AddPage(from.Pages[i]);
        }
    }
}
