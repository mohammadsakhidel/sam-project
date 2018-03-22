package com.ramanco.samandroid.utils;

import android.text.Html;
import android.text.Spannable;
import android.text.SpannableString;
import android.text.Spanned;
import android.text.TextUtils;
import android.text.style.URLSpan;
import android.text.util.Linkify;
import android.util.Base64;

import com.google.gson.Gson;
import com.google.gson.reflect.TypeToken;

import java.lang.reflect.Type;
import java.util.ArrayList;
import java.util.List;
import java.util.Locale;
import java.util.Map;
import java.util.regex.Matcher;
import java.util.regex.Pattern;

import com.ramanco.samandroid.consts.Patterns;

public class TextUtility {

    public static String concat(String[] strList, char sep, boolean addSpace) {
        StringBuilder res = new StringBuilder();
        for (int i = 0; i < strList.length; i++) {
            String tmp = strList[i] + (i < strList.length - 1 ? sep + (addSpace ? " " : "") : "");
            res = res.append(tmp);
        }
        return res.toString();
    }

    public static List<String> removeEmpties(List<String> list) {
        List<String> res = new ArrayList<>();
        for (String s : list) {
            if (!TextUtils.isEmpty(s)) {
                res.add(s);
            }
        }
        return res;
    }

    public static boolean isEmailValid(String email) {
        String rgx = Patterns.EMAIL;
        Pattern pattern = Pattern.compile(rgx, Pattern.CASE_INSENSITIVE);
        return pattern.matcher(email).matches();
    }

    public static boolean isPasswordValid(String password) {
        String rgx = Patterns.PASSWORD;
        Pattern pattern = Pattern.compile(rgx, Pattern.CASE_INSENSITIVE);
        return pattern.matcher(password).matches();
    }

    public static String toBase64(String text) throws Exception {
        byte[] data = text.getBytes("UTF-8");
        return Base64.encodeToString(data, Base64.DEFAULT);
    }

    public static String fromBase64(String base64) throws Exception {
        byte[] data = Base64.decode(base64, Base64.DEFAULT);
        return new String(data, "UTF-8");
    }

    public static Map<String, String> parseJWTPayload(String token) throws Exception {

        String[] splitArr = token.split("\\.");
        String payloadBase64 = splitArr[1];

        String payloadJson = fromBase64(payloadBase64);
        Gson gson = new Gson();
        Type type = new TypeToken<Map<String, String>>() {
        }.getType();
        return gson.fromJson(payloadJson, type);

    }

    public static String getSizeText(int sizeInKilobytes) {
        if (sizeInKilobytes < 1024)
            return String.format(Locale.getDefault(), "%d %s", sizeInKilobytes, "KB");
        else if (sizeInKilobytes < 1024 * 1024) {
            double megaBytes = (double) sizeInKilobytes / 1024;
            return String.format(Locale.getDefault(), "%.2f %s", megaBytes, "MB");
        } else {
            double gigaBytes = (double) sizeInKilobytes / (1024 * 1024);
            return String.format(Locale.getDefault(), "%.2f %s", gigaBytes, "GB");
        }
    }

    public static boolean isValid(String regex, String text) {
        Pattern p = Pattern.compile(regex);
        return p.matcher(text).matches();
    }

    public static Spanned fromHtml(String html) {
        Spanned result;
        if (android.os.Build.VERSION.SDK_INT >= android.os.Build.VERSION_CODES.N) {
            result = Html.fromHtml(html, Html.FROM_HTML_MODE_LEGACY);
        } else {
            result = Html.fromHtml(html);
        }
        return result;
    }

    public static String plainToHtml(String content) {
        String html;
        html = content.replaceAll("\\n", "<br />");
        return html;
    }

    public static String textToAnchor(String anchorText, String colorHex) {
        return String.format("<a><font color=\"%s\">%s</font></a>", colorHex, anchorText);
    }

    public static Spannable linkifyHtml(String html, int linkifyMask) {
        Spanned text = fromHtml(html);
        URLSpan[] currentSpans = text.getSpans(0, text.length(), URLSpan.class);

        SpannableString buffer = new SpannableString(text);
        Linkify.addLinks(buffer, linkifyMask);

        for (URLSpan span : currentSpans) {
            int end = text.getSpanEnd(span);
            int start = text.getSpanStart(span);
            buffer.setSpan(span, start, end, 0);
        }
        return buffer;
    }

    public static int findPastOccurrence(String text, char ch, int position) {
        int index = -1;
        if (position > 0) {
            position--;
            while (position >= 0) {

                if (ch == text.charAt(position))
                    return position;

                position--;
            }
        }
        return index;
    }

    public static int findNextOccurrence(String text, char ch, int position) {
        int index = -1;
        if (position < text.length() - 1) {
            position++;
            while (position < text.length()) {

                if (ch == text.charAt(position))
                    return position;

                position++;
            }
        }
        return index;
    }

    public static String encodeEmail(String email) {
        email = replaceLastOccurrence(email, "@", "[at]");
        email = replaceLastOccurrence(email, ".", "[dot]");
        return email;
    }

    public static String decodeEmail(String encodedEmail) {
        encodedEmail = replaceLastOccurrence(encodedEmail, "[at]", "@");
        encodedEmail = replaceLastOccurrence(encodedEmail, "[dot]", ".");
        return encodedEmail;
    }

    public static String replaceLastOccurrence(String src, String oldStr, String newStr) {
        int plc = src.lastIndexOf(oldStr);
        if (plc < 0)
            return src;
        return src.substring(0, plc) + newStr + src.substring(plc + oldStr.length());
    }

    public static String bytesToString(byte[] bytes) {
        /*StringBuilder sb = new StringBuilder();

        int index = 0;
        final int blockSize = 10;
        int blocksCount = bytes.length / blockSize;

        for (int b = 0; b < blocksCount; b++) {
            byte[] block = new byte[blockSize];
            System.arraycopy(bytes, b * blockSize, block, 0, blockSize);
            sb.append(new String(block));
        }

        if (bytes.length % blockSize > 0) {
            byte[] block = new byte[bytes.length % blockSize];
            System.arraycopy(bytes, blocksCount * blockSize, block, 0, bytes.length % blockSize);
            sb.append(new String(block));
        }

        return sb.toString();*/
        return new String(bytes);
    }
}
