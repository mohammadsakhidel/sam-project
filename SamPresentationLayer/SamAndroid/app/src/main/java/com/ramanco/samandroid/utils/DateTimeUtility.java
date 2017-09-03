package com.ramanco.samandroid.utils;

import org.joda.time.DateTime;
import org.joda.time.DateTimeZone;
import org.joda.time.format.DateTimeFormat;
import org.joda.time.format.DateTimeFormatter;
import java.text.ParseException;
import java.util.Date;

public class DateTimeUtility {

    public static Date getNow() {
        return DateTime.now().toDate();
    }

    public static Date getUTCNow() {
        return DateTime.now(DateTimeZone.UTC).toDate();
    }

    public static Date fromUtcString(String dtString, String dtFormat) throws ParseException {
        DateTimeFormatter dateTimeFormatter = DateTimeFormat.forPattern(dtFormat).withZoneUTC();
        DateTime dt = DateTime.parse(dtString, dateTimeFormatter);
        return dt.toDate();
    }

    public static String toUtcString(Date date, String dtFormat) {
        DateTime dateTime = new DateTime(date);
        DateTimeFormatter dateTimeFormatter = DateTimeFormat.forPattern(dtFormat).withZoneUTC();
        return dateTime.toString(dateTimeFormatter);
    }

}