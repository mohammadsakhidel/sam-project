package com.ramanco.samandroid.utils;

import android.content.Context;

import com.ramanco.samandroid.R;
import com.ramanco.samandroid.objects.KeyValuePair;

import org.w3c.dom.Document;
import org.w3c.dom.Element;
import org.w3c.dom.Node;
import org.w3c.dom.NodeList;
import org.xml.sax.SAXException;

import java.io.IOException;
import java.io.InputStream;
import java.util.ArrayList;
import java.util.Collections;
import java.util.List;

import javax.xml.parsers.DocumentBuilder;
import javax.xml.parsers.DocumentBuilderFactory;
import javax.xml.parsers.ParserConfigurationException;

public class CityUtil {

    public static List<KeyValuePair> getAllCities(Context context) throws ParserConfigurationException, IOException, SAXException {

        List<KeyValuePair> pairs = new ArrayList<>();
        InputStream is = context.getResources().openRawResource(R.raw.ircities);
        try {
            DocumentBuilderFactory factory = DocumentBuilderFactory.newInstance();
            DocumentBuilder builder = factory.newDocumentBuilder();
            Document dom = builder.parse(is);
            Element root = dom.getDocumentElement();
            NodeList nodes = root.getElementsByTagName("City");
            for (int i = 0; i < nodes.getLength(); i++) {
                Node node = nodes.item(i);
                if (node.getNodeType() == Node.ELEMENT_NODE) {
                    Element element = (Element) node;
                    String cityID = element.getAttribute("ID");
                    String cityName = element.getAttribute("Name");
                    pairs.add(new KeyValuePair(cityID, cityName));
                }
            }
            Collections.sort(pairs);
        } finally {
            is.close();
        }
        return pairs;

    }

}
